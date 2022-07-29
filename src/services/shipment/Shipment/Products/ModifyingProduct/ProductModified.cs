namespace Shipment.Products.ModifyingProduct;

public record ProductModified(
    Guid ProductId,
    string Sku,
    string Name,
    string Description
);