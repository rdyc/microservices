using FW.Core.Commands;
using FW.Core.Pagination;
using FW.Core.Queries;
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
    internal static async Task<IResult> Products(
        int index,
        int size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await query.SendAsync<GetProducts, IListPaged<ProductShortInfo>>(
                new GetProducts(index, size), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Retrieve product", OperationId = "get_product", Tags = new[] { "Product" })]
    internal static async Task<IResult> Product(
        [FromRoute] Guid productId,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await query.SendAsync<GetProductById, ProductDetail>(
                new GetProductById(productId), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Retrieve product histories", OperationId = "get_history", Tags = new[] { "Product" })]
    internal static async Task<IResult> Histories(
        [FromRoute] Guid productId,
        int index,
        int size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await query.SendAsync<GetProductHistory, IListPaged<ProductHistory>>(
                GetProductHistory.Create(productId, index, size), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Register a new product", OperationId = "register", Tags = new[] { "Product" })]
    internal static async Task<IResult> Create(
        [FromBody] ProductCreateRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var id = Guid.NewGuid();
            var (sku, name, description) = request;

            await command.SendAsync(new RegisterProduct(id, sku, name, description), cancellationToken);

            return Results.Created(string.Empty, id);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Modify existing product", OperationId = "modify", Tags = new[] { "Product" })]
    internal static async Task<IResult> Update(
        [FromRoute] Guid productId,
        [FromBody] ProductModifyRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var (name, code, symbol) = request;

            await command.SendAsync(new ModifyProduct(productId, name, code, symbol), cancellationToken);

            return Results.Accepted(string.Empty, productId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Add product attribute", OperationId = "add_attribute", Tags = new[] { "Product" })]
    internal static async Task<IResult> AddAttribute(
        [FromRoute] Guid productId,
        [FromBody] AddProductAttributeRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var (attributeId, value) = request;

            await command.SendAsync(new AddProductAttribute(productId, attributeId, value), cancellationToken);

            return Results.Accepted(string.Empty, productId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Remove product attribute", OperationId = "remove_attribute", Tags = new[] { "Product" })]
    internal static async Task<IResult> RemoveAttribute(
        [FromRoute] Guid productId,
        [FromRoute] Guid attributeId,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await command.SendAsync(new RemoveProductAttribute(productId, attributeId), cancellationToken);

            return Results.Accepted(string.Empty, productId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Update product pricing", OperationId = "update_price", Tags = new[] { "Product" })]
    internal static async Task<IResult> UpdatePrice(
        [FromRoute] Guid productId,
        [FromBody] UpdateProductPriceRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var (currencyId, price) = request;

            await command.SendAsync(new UpdateProductPrice(productId, currencyId, price), cancellationToken);

            return Results.Accepted(string.Empty, productId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Update product stock", OperationId = "update_stock", Tags = new[] { "Product" })]
    internal static async Task<IResult> UpdateStock(
        [FromRoute] Guid productId,
        [FromBody] UpdateProductStockRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await command.SendAsync(new UpdateProductStock(productId, request.Stock), cancellationToken);

            return Results.Accepted(string.Empty, productId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Remove existing product", OperationId = "remove", Tags = new[] { "Product" })]
    internal static async Task<IResult> Delete(
        [FromRoute] Guid productId,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await command.SendAsync(new RemoveProduct(productId), cancellationToken);

            return Results.NoContent();
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }
}