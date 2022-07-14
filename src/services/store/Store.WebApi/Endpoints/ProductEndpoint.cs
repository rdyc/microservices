using MediatR;
using Microsoft.AspNetCore.Mvc;
using Store.Products.AddingAttribute;
using Store.Products.GettingProductById;
using Store.Products.GettingProductHistory;
using Store.Products.GettingProducts;
using Store.Products.ModifyingProduct;
using Store.Products.RegisteringProduct;
using Store.Products.RemovingAttribute;
using Store.Products.RemovingProduct;
using Store.Products.UpdatingPrice;
using Store.Products.UpdatingStock;
using Store.WebApi.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace Store.WebApi.Endpoints;

public static class ProductEndpoint
{
    [SwaggerOperation(Summary = "Retrieve all products", OperationId = "get_products", Tags = new[] { "Product" })]
    internal static async Task<IResult> Products(int index, int size, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await mediator.Send(new GetProducts(index, size), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Retrieve product", OperationId = "get_product", Tags = new[] { "Product" })]
    internal static async Task<IResult> Product(Guid productId, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await mediator.Send(new GetProductById(productId), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Retrieve product histories", OperationId = "get_history", Tags = new[] { "Product" })]
    internal static async Task<IResult> Histories(Guid productId, int index, int size, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await mediator.Send(GetProductHistory.Create(productId, index, size), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Register a new product", OperationId = "register", Tags = new[] { "Product" })]
    internal static async Task<IResult> Create([FromBody] ProductCreateRequest request, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var id = Guid.NewGuid();
            var (sku, name, description) = request;

            await mediator.Send(new RegisterProduct(id, sku, name, description), cancellationToken);

            return Results.Created(string.Empty, id);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Modify existing product", OperationId = "modify", Tags = new[] { "Product" })]
    internal static async Task<IResult> Update(Guid productId, [FromBody] ProductModifyRequest request, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var (name, code, symbol) = request;

            await mediator.Send(new ModifyProduct(productId, name, code, symbol), cancellationToken);

            return Results.Accepted(string.Empty, productId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Add product attribute", OperationId = "add_attribute", Tags = new[] { "Product" })]
    internal static async Task<IResult> AddAttribute(Guid productId, [FromBody] AddProductAttributeRequest request, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var (attributeId, value) = request;

            await mediator.Send(new AddAttribute(productId, attributeId, value), cancellationToken);

            return Results.Accepted(string.Empty, productId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Remove product attribute", OperationId = "remove_attribute", Tags = new[] { "Product" })]
    internal static async Task<IResult> RemoveAttribute(Guid productId, Guid attributeId, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await mediator.Send(new RemoveAttribute(productId, attributeId), cancellationToken);

            return Results.Accepted(string.Empty, productId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Update product pricing", OperationId = "update_price", Tags = new[] { "Product" })]
    internal static async Task<IResult> UpdatePrice(Guid productId, [FromBody] UpdateProductPriceRequest request, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var (currencyId, price) = request;

            await mediator.Send(new UpdatePrice(productId, currencyId, price), cancellationToken);

            return Results.Accepted(string.Empty, productId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Update product stock", OperationId = "update_stock", Tags = new[] { "Product" })]
    internal static async Task<IResult> UpdateStock(Guid productId, [FromBody] UpdateProductStockRequest request, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await mediator.Send(new UpdateStock(productId, request.Stock), cancellationToken);

            return Results.Accepted(string.Empty, productId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Remove existing product", OperationId = "remove", Tags = new[] { "Product" })]
    internal static async Task<IResult> Delete(Guid productId, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await mediator.Send(new RemoveProduct(productId), cancellationToken);

            return Results.NoContent();
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }
}