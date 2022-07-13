using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using Store.Lookup.Attributes;
using Store.Lookup.Currencies;
using Store.Products.AddingAttribute;
using Store.Products.ModifyingProduct;
using Store.Products.RegisteringProduct;
using Store.Products.RemovingAttribute;
using Store.Products.RemovingProduct;
using Store.Products.SellingProduct;
using Store.Products.UpdatingPrice;
using Store.Products.UpdatingStock;

namespace Store.Products.GettingProductById;

[BsonCollection("product_detail")]
public record ProductDetail : Document
{
    [BsonElement("sku")]
    public string Sku { get; set; } = default!;

    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("desc")]
    public string Description { get; set; } = default!;

    [BsonElement("status")]
    public ProductStatus Status { get; set; } = default!;

    [BsonElement("attributes")]
    public IList<ProductDetailAttribute> Attributes { get; private set; } = default!;

    [BsonElement("currency")]
    public Currency Currency { get; set; } = default!;

    [BsonElement("price")]
    public decimal Price { get; set; } = default!;

    [BsonElement("stock")]
    public int Stock { get; set; } = default!;

    [BsonElement("sold")]
    public int Sold { get; set; } = default!;

    [BsonElement("version")]
    public int Version { get; set; }

    [BsonElement("position")]
    public ulong LastProcessedPosition { get; set; }
}

public record ProductDetailAttribute
{
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

public class ProductDetailProjection
{
    public static ProductDetail Handle(EventEnvelope<ProductRegistered> eventEnvelope)
    {
        var (id, sku, name, desc, status) = eventEnvelope.Data;

        return new ProductDetail
        {
            Id = id,
            Sku = sku,
            Name = name,
            Description = desc,
            Status = status,
            Version = 0,
            LastProcessedPosition = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<ProductModified> eventEnvelope, ProductDetail view)
    {
        if (view.LastProcessedPosition >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, sku, name, desc) = eventEnvelope.Data;

        view.Sku = sku;
        view.Name = name;
        view.Description = desc;
        view.Version++;
        view.LastProcessedPosition = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<PriceChanged> eventEnvelope, ProductDetail view)
    {
        if (view.LastProcessedPosition >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, currency, price) = eventEnvelope.Data;

        view.Currency = currency;
        view.Price = price;
        view.Version++;
        view.LastProcessedPosition = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<StockChanged> eventEnvelope, ProductDetail view)
    {
        if (view.LastProcessedPosition >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, stock) = eventEnvelope.Data;

        view.Stock = stock;
        view.Version++;
        view.LastProcessedPosition = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ProductSold> eventEnvelope, ProductDetail view)
    {
        if (view.LastProcessedPosition >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, quantity) = eventEnvelope.Data;

        view.Sold += quantity;
        view.Version++;
        view.LastProcessedPosition = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<AttributeAdded> eventEnvelope, ProductDetail view)
    {
        if (view.LastProcessedPosition >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, id, name, type, unit, value) = eventEnvelope.Data;

        view.Attributes.Add(new ProductDetailAttribute
        {
            Id = id,
            Name = name,
            Type = type,
            Unit = unit,
            Value = value
        });
        view.Version++;
        view.LastProcessedPosition = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<RemovingAttribute.AttributeRemoved> eventEnvelope, ProductDetail view)
    {
        if (view.LastProcessedPosition >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, id, name, type, unit, value) = eventEnvelope.Data;

        if (view.Attributes.Any(e => e.Id.Equals(id) && e.Value.Equals(value)))
        {
            view.Attributes.Remove(new ProductDetailAttribute
            {
                Id = id,
                Name = name,
                Type = type,
                Unit = unit,
                Value = value
            });
        }

        view.Version++;
        view.LastProcessedPosition = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ProductRemoved> eventEnvelope, ProductDetail view)
    {
        if (view.LastProcessedPosition >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = ProductStatus.Discontinue;
        view.Version++;
        view.LastProcessedPosition = eventEnvelope.Metadata.LogPosition;
    }
}