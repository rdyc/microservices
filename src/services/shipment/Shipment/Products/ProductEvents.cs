namespace Shipment.Products;

public record ProductRegistered(Guid Id, string Sku, string Name, string Description, ProductStatus Status);
public record ProductModified(Guid Id, string Sku, string Name, string Description);
public record ProductStockChanged(Guid Id, int Stock);
public record ProductRemoved(Guid Id);

public enum ProductStatus
{
    Available,
    Discontinue
}