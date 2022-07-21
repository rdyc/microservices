using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Cart.Products;

[BsonCollection("product")]
public record Product : Document
{
    [BsonElement("sku")]
    public string Sku { get; set; } = default!;

    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("description")]
    public string Description { get; set; } = default!;

    [BsonElement("attributes")]
    public IList<ProductAttribute> Attributes { get; set; } = default!;

    [BsonElement("status")]
    public ProductStatus Status { get; set; } = default!;

    [BsonElement("currency")]
    public ProductCurrency Currency { get; set; } = default!;

    [BsonElement("price")]
    public decimal Price { get; set; } = default!;

    [BsonElement("stock")]
    public int Stock { get; set; } = default!;

    [BsonElement("version")]
    public ulong Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
}

public record ProductCurrency
{
    private ProductCurrency(
        Guid id,
        string name,
        string code,
        string symbol)
    {
        Id = id;
        Name = name;
        Code = code;
        Symbol = symbol;
    }

    public static ProductCurrency Create(
        Guid id,
        string name,
        string code,
        string symbol
    ) => new(id, name, code, symbol);

    [BsonElement("id")]
    public Guid Id { get; set; } = default!;

    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("code")]
    public string Code { get; set; } = default!;

    [BsonElement("symbol")]
    public string Symbol { get; set; } = default!;
}

public record ProductAttribute
{
    private ProductAttribute(Guid id, string name, AttributeType type, string unit, string value)
    {
        Id = id;
        Name = name;
        Type = type;
        Unit = unit;
        Value = value;
    }

    public static ProductAttribute Create(Guid id, string name, AttributeType type, string unit, string value) =>
        new(id, name, type, unit, value);

    [BsonElement("id")]
    public Guid Id { get; set; } = default!;

    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("type")]
    public AttributeType Type { get; set; } = default!;

    [BsonElement("unit")]
    public string Unit { get; set; } = default!;

    [BsonElement("value")]
    public string Value { get; set; } = default!;
}


public class ProductProjection
{
    public static Product Handle(EventEnvelope<ProductRegistered> eventEnvelope)
    {
        var (id, sku, name, description, status) = eventEnvelope.Data;

        return new Product
        {
            Id = id,
            Sku = sku,
            Name = name,
            Description = description,
            Status = status,
            Version = eventEnvelope.Metadata.StreamPosition,
            Position = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<ProductModified> eventEnvelope, Product view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, sku, name, description) = eventEnvelope.Data;

        view.Sku = sku;
        view.Name = name;
        view.Description = description;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ProductAttributeAdded> eventEnvelope, Product view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        if (view.Attributes is null)
            view.Attributes = new List<ProductAttribute>();

        var (_, id, name, type, unit, value) = eventEnvelope.Data;

        view.Attributes.Add(ProductAttribute.Create(id, name, type, unit, value));
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ProductAttributeRemoved> eventEnvelope, Product view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        if (view.Attributes != null)
        {
            var (_, id, name, type, unit, value) = eventEnvelope.Data;

            var removedAttribute = view.Attributes
                .SingleOrDefault(e => e.Id.Equals(id) && e.Value.Equals(value));

            if (removedAttribute != null)
            {
                view.Attributes.Remove(removedAttribute);
            }
        }

        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ProductPriceChanged> eventEnvelope, Product view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, currency, price) = eventEnvelope.Data;

        view.Currency = ProductCurrency.Create(currency.Id, currency.Name, currency.Code, currency.Symbol);
        view.Price = price;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ProductStockChanged> eventEnvelope, Product view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, stock) = eventEnvelope.Data;

        view.Stock = stock;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ProductRemoved> eventEnvelope, Product view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = ProductStatus.Discontinue;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }
}