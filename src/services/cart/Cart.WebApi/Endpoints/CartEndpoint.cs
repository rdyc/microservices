using Cart.ShoppingCarts;
using Cart.ShoppingCarts.AddingProduct;
using Cart.ShoppingCarts.CancelingCart;
using Cart.ShoppingCarts.ConfirmingCart;
using Cart.ShoppingCarts.GettingCartAtVersion;
using Cart.ShoppingCarts.GettingCartById;
using Cart.ShoppingCarts.GettingCartHistory;
using Cart.ShoppingCarts.GettingCarts;
using Cart.ShoppingCarts.OpeningCart;
using Cart.ShoppingCarts.RemovingProduct;
using Cart.WebApi.Requests;
using FW.Core.Commands;
using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.WebApi;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cart.WebApi.Endpoints;

internal static class CartEndpoint
{
    [SwaggerOperation(Summary = "Retrieve all carts", OperationId = "carts", Tags = new[] { "Cart" })]
    internal static async Task<IResult> Carts(
        [FromQuery] int? page,
        [FromQuery] int? size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetCarts, IListPaged<ShoppingCartShortInfo>>(
            GetCarts.Create(page, size), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Retrieve cart", OperationId = "cart", Tags = new[] { "Cart" })]
    internal static async Task<IResult> CartDetails(
        [FromRoute] Guid cartId,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetCartById, ShoppingCartDetails>(
            GetCartById.Create(cartId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Retrieve cart at version", OperationId = "cart_version", Tags = new[] { "Cart" })]
    internal static async Task<IResult> CartAtVersion(
        [FromRoute] Guid cartId,
        [FromRoute] ulong version,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetCartAtVersion, ShoppingCart>(
            GetCartAtVersion.Create(cartId, version), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Retrieve cart histories", OperationId = "histories", Tags = new[] { "Cart" })]
    internal static async Task<IResult> Histories(
        [FromRoute] Guid cartId,
        [FromQuery] int? page,
        [FromQuery] int? size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetCartHistory, IListPaged<ShoppingCartHistory>>(
            GetCartHistory.Create(cartId, page, size), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Open a new cart", OperationId = "open", Tags = new[] { "Cart" })]
    internal static async Task<IResult> Open(
        [FromBody] OpenShoppingCartRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var cartId = Guid.NewGuid();
        var task = command.SendAsync(OpenShoppingCart.Create(cartId, request.ClientId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Created(string.Empty, cartId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Add product", OperationId = "add_product", Tags = new[] { "Cart" })]
    internal static async Task<IResult> Add(
        [FromRoute] Guid cartId,
        [FromBody] AddProductRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var (productId, quantity) = request;
        var task = command.SendAsync(AddProductCart.Create(cartId, productId, quantity), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Accepted(string.Empty, cartId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Remove product", OperationId = "remove_product", Tags = new[] { "Cart" })]
    internal static async Task<IResult> Remove(
        [FromRoute] Guid cartId,
        [FromBody] RemoveProductRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = command.SendAsync(RemoveProductCart.Create(cartId, request.ProductId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Accepted(string.Empty, cartId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Canceling cart", OperationId = "cancel", Tags = new[] { "Cart" })]
    internal static async Task<IResult> Cancel(
        [FromRoute] Guid cartId,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = command.SendAsync(CancelShoppingCart.Create(cartId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Accepted(string.Empty, cartId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "confirm cart", OperationId = "confirm", Tags = new[] { "Cart" })]
    internal static async Task<IResult> Confirm(
        [FromRoute] Guid cartId,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = command.SendAsync(ConfirmShoppingCart.Create(cartId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Accepted(string.Empty, cartId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }
}