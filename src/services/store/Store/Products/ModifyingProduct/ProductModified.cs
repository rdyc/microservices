using FW.Core.Events;

namespace Store.Products.ModifyingProduct;

public record ProductModified(
    Guid Id,
    string Sku,
    string Name,
    string Description
) : IExternalEvent
{
    public static ProductModified Create(Guid id, string sku, string name, string description) =>
        new(id, sku, name, description);
}