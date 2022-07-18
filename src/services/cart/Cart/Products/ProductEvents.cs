namespace Cart.Products;

public record ProductRegistered(Guid Id, string Sku, string Name, ProductStatus Status);
public record ProductModified(Guid Id, string Sku, string Name);
public record ProductRemoved(Guid Id);