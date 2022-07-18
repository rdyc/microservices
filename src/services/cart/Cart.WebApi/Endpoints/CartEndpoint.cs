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
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cart.WebApi.Endpoints;

public static class CartEndpoint
{
    [SwaggerOperation(Summary = "Retrieve all carts", OperationId = "get_carts", Tags = new[] { "Cart" })]
    internal static async Task<IResult> Carts(
        int index,
        int size,
        [FromServices] IMediator mediator,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await mediator.Send(new GetCarts(index, size), cancellationToken);

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
        [FromServices] IMediator mediator,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await mediator.Send(GetCartById.Create(cartId), cancellationToken);

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
        [FromServices] IMediator mediator,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await mediator.Send(GetCartAtVersion.Create(cartId, version), cancellationToken);

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
        [FromServices] IMediator mediator,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await mediator.Send(GetCartHistory.Create(cartId, index, size), cancellationToken);

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
        [FromServices] IMediator mediator,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var id = Guid.NewGuid();

            await mediator.Send(OpenShoppingCart.Create(id, request.ClientId), cancellationToken);

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
        [FromServices] IMediator mediator,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var (productId, quantity) = request.Product;

            await mediator.Send(AddProduct.Create(cartId, productId, quantity), cancellationToken);

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
        [FromServices] IMediator mediator,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await mediator.Send(RemoveProduct.Create(cartId, request.Product.ProductId), cancellationToken);

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
        [FromServices] IMediator mediator,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await mediator.Send(new CancelShoppingCart(cartId), cancellationToken);

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
        [FromServices] IMediator mediator,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await mediator.Send(new ConfirmShoppingCart(cartId), cancellationToken);

            return Results.NoContent();
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }
}