namespace Shipment.Products;

public record ProductRegistered(Guid ProductId, string Sku, string Name, string Description, ProductStatus Status);
public record ProductModified(Guid ProductId, string Sku, string Name, string Description);
public record ProductAttributeAdded(Guid ProductId, Guid AttributeId, string Name, AttributeType Type, string Unit, string Value);
public record ProductPriceChanged(Guid ProductId, ProductCurrency Currency, decimal Price);
public record ProductStockChanged(Guid ProductId, int Stock);
public record ProductShipped(Guid ProductId, int Quantity);
public record ProductRemoved(Guid ProductId);
public record ProductCurrency(Guid Id, string Name, string Code, string Symbol);
public enum ProductStatus
{
    Available,
    Discontinue
}

public enum AttributeType
{
    Text,
    Number,
    Decimal
}