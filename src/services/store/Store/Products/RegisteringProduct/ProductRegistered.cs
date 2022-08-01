using FW.Core.Events;

namespace Store.Products.RegisteringProduct;

public record ProductRegistered(
    Guid Id,
    string Sku,
    string Name,
    string Description,
    ProductStatus Status
) : IExternalEvent
{
    public static ProductRegistered Create(
        Guid id,
        string sku,
        string name,
        string description,
        ProductStatus status
    ) => new(id, sku, name, description, status);
}