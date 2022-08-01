namespace Cart.Products.RegisteringProduct;

public record ProductRegistered(
    Guid Id,
    string Sku,
    string Name,
    string Description,
    ProductStatus Status
);