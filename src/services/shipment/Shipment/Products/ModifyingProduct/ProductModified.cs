namespace Shipment.Products.ModifyingProduct;

public record ProductModified(
    Guid Id,
    string Sku,
    string Name,
    string Description
);