using Cart.ShoppingCarts.AddingProduct;
using Cart.ShoppingCarts.CancelingCart;
using Cart.ShoppingCarts.ConfirmingCart;
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
    internal static async Task<IResult> Carts(int index, int size, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
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
    internal static async Task<IResult> Cart(Guid cartId, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await mediator.Send(new GetCartById(cartId), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Retrieve cart histories", OperationId = "get_history", Tags = new[] { "Cart" })]
    internal static async Task<IResult> Histories(Guid cartId, int index, int size, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
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
    internal static async Task<IResult> Open([FromBody] OpenShoppingCartRequest request, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
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
    internal static async Task<IResult> ProductAdd(Guid cartId, [FromBody] AddProductRequest request, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            // var (attributeId, value) = request;

            await mediator.Send(AddProduct.Create(cartId, null), cancellationToken);

            return Results.Accepted(string.Empty, cartId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Remove product", OperationId = "remove_product", Tags = new[] { "Cart" })]
    internal static async Task<IResult> RemoveProduct(Guid cartId, RemoveProductRequest request, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await mediator.Send(new RemoveProduct(cartId, null), cancellationToken);

            return Results.Accepted(string.Empty, cartId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Canceling cart", OperationId = "cancel", Tags = new[] { "Cart" })]
    internal static async Task<IResult> Cancel(Guid cartId, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
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
    internal static async Task<IResult> Confirm(Guid cartId, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
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