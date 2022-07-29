namespace Shipment.Products.RegisteringProduct;

public record ProductRegistered(
    Guid ProductId,
    string Sku,
    string Name,
    string Description,
    ProductStatus Status
);