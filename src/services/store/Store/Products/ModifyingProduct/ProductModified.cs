using FW.Core.Events;

namespace Store.Products.ModifyingProduct;

public record ProductModified(
    Guid ProductId,
    string Sku,
    string Name,
    string Description
) : IExternalEvent
{
    public static ProductModified Create(
        Guid productId,
        string sku,
        string name,
        string description
    ) => new(productId, sku, name, description);
}