using FW.Core.Commands;
using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.WebApi;
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
        var task = query.SendAsync<GetProducts, IListPaged<ProductShortInfo>>(
            GetProducts.Create(index, size), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Retrieve product", OperationId = "get_product", Tags = new[] { "Product" })]
    internal static async Task<IResult> Product(
        [FromRoute] Guid productId,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetProductById, ProductDetail>(
            GetProductById.Create(productId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
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
        var task = query.SendAsync<GetProductHistory, IListPaged<ProductHistory>>(
            GetProductHistory.Create(productId, index, size), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Register a new product", OperationId = "register", Tags = new[] { "Product" })]
    internal static async Task<IResult> Create(
        [FromBody] ProductCreateRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var productId = Guid.NewGuid();
        var (sku, name, description) = request;
        var task = command.SendAsync(
            RegisterProduct.Create(productId, sku, name, description), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Created(string.Empty, productId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
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
        var (name, code, symbol) = request;
        var task = command.SendAsync(
            ModifyProduct.Create(productId, name, code, symbol), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Accepted(string.Empty, productId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
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
        var (attributeId, value) = request;
        var task = command.SendAsync(
            AddProductAttribute.Create(productId, attributeId, value), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Accepted(string.Empty, productId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
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
        var task = command.SendAsync(
            RemoveProductAttribute.Create(productId, attributeId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Accepted(string.Empty, productId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
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
        var (currencyId, price) = request;
        var task = command.SendAsync(
            UpdateProductPrice.Create(productId, currencyId, price), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Accepted(string.Empty, productId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
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
        var task = command.SendAsync(
            UpdateProductStock.Create(productId, request.Stock), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Accepted(string.Empty, productId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Remove existing product", OperationId = "remove", Tags = new[] { "Product" })]
    internal static async Task<IResult> Delete(
        [FromRoute] Guid productId,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = command.SendAsync(
            RemoveProduct.Create(productId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.NoContent(),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }
}