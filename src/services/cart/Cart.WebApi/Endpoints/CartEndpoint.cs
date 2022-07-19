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
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cart.WebApi.Endpoints;

internal static class CartEndpoint
{
    [SwaggerOperation(Summary = "Retrieve all carts", OperationId = "get_carts", Tags = new[] { "Cart" })]
    internal static async Task<IResult> Carts(
        int index,
        int size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await query.SendAsync<GetCarts, IListPaged<ShoppingCartShortInfo>>(
                new GetCarts(index, size), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Retrieve cart", OperationId = "get_cart", Tags = new[] { "Cart" })]
    internal static async Task<IResult> CartDetails(
        [FromRoute] Guid cartId,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await query.SendAsync<GetCartById, ShoppingCartDetails>(
                GetCartById.Create(cartId), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Retrieve cart at version", OperationId = "get_cart", Tags = new[] { "Cart" })]
    internal static async Task<IResult> CartAtVersion(
        [FromRoute] Guid cartId,
        [FromRoute] ulong version,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await query.SendAsync<GetCartAtVersion, ShoppingCart>(
                GetCartAtVersion.Create(cartId, version), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Retrieve cart histories", OperationId = "get_history", Tags = new[] { "Cart" })]
    internal static async Task<IResult> Histories(
        [FromRoute] Guid cartId,
        int index,
        int size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await query.SendAsync<GetCartHistory, IListPaged<ShoppingCartHistory>>(
                GetCartHistory.Create(cartId, index, size), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Open a new cart", OperationId = "open", Tags = new[] { "Cart" })]
    internal static async Task<IResult> Open(
        [FromBody] OpenShoppingCartRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var id = Guid.NewGuid();

            await command.SendAsync(OpenShoppingCart.Create(id, request.ClientId), cancellationToken);

            return Results.Created(string.Empty, id);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
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

        try
        {
            var (productId, quantity) = request.Product;

            await command.SendAsync(AddProductCart.Create(cartId, productId, quantity), cancellationToken);

            return Results.Accepted(string.Empty, cartId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
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

        try
        {
            await command.SendAsync(RemoveProductCart.Create(cartId, request.Product.ProductId), cancellationToken);

            return Results.Accepted(string.Empty, cartId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Canceling cart", OperationId = "cancel", Tags = new[] { "Cart" })]
    internal static async Task<IResult> Cancel(
        [FromRoute] Guid cartId,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await command.SendAsync(new CancelShoppingCart(cartId), cancellationToken);

            return Results.NoContent();
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "confirm cart", OperationId = "confirm", Tags = new[] { "Cart" })]
    internal static async Task<IResult> Confirm(
        [FromRoute] Guid cartId,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await command.SendAsync(new ConfirmShoppingCart(cartId), cancellationToken);

            return Results.NoContent();
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }
}