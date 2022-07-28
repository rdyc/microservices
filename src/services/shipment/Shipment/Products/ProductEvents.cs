namespace Shipment.Products;

public record ProductRegistered(Guid Id, string Sku, string Name, string Description, ProductStatus Status);
public record ProductModified(Guid Id, string Sku, string Name, string Description);
public record ProductAttributeAdded(Guid Id, Guid AttributeId, string Name, AttributeType Type, string Unit, string Value);
public record ProductPriceChanged(Guid Id, ProductCurrency Currency, decimal Price);
public record ProductStockChanged(Guid Id, int Stock);
public record ProductShipped(Guid ProductId, int Quantity);
public record ProductRemoved(Guid Id);
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