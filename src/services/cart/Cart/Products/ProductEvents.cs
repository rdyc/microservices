namespace Cart.Products;

public record ProductRegistered(Guid Id, string Sku, string Name, string Description, ProductStatus Status);
public record ProductModified(Guid Id, string Sku, string Name, string Description);
public record ProductAttributeAdded(Guid Id, Guid AttributeId, string Name, AttributeType Type, string Unit, string Value);
public record ProductAttributeRemoved(Guid Id, Guid AttributeId, string Name, AttributeType Type, string Unit, string Value);
public record ProductPriceChanged(Guid Id, Currency Currency, decimal Price);
public record ProductStockChanged(Guid Id, int Stock);
public record ProductRemoved(Guid Id);
public record Currency(Guid Id, string Name, string Code, string Symbol);

public enum AttributeType
{
    Text,
    Number,
    Decimal
}

public enum ProductStatus
{
    Available,
    Discontinue
}