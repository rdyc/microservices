using FW.Core.Events;

namespace Store.Products.RegisteringProduct;

public record ProductRegistered(
    Guid ProductId,
    string Sku,
    string Name,
    string Description,
    ProductStatus Status
) : IExternalEvent
{
    public static ProductRegistered Create(
        Guid productId,
        string sku,
        string name,
        string description,
        ProductStatus status
    ) => new(productId, sku, name, description, status);
}