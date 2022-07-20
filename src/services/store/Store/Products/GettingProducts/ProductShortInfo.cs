using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using Store.Lookup.Currencies;
using Store.Products.ModifyingProduct;
using Store.Products.RegisteringProduct;
using Store.Products.RemovingProduct;
using Store.Products.SellingProduct;
using Store.Products.UpdatingPrice;
using Store.Products.UpdatingStock;

namespace Store.Products.GettingProducts;

[BsonCollection("product_shortinfo")]
public record ProductShortInfo : Document
{
    [BsonElement("sku")]
    public string Sku { get; set; } = default!;

    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("desc")]
    public string Description { get; set; } = default!;

    [BsonElement("status")]
    public ProductStatus Status { get; set; } = default!;

    [BsonElement("currency")]
    public ProductCurrency Currency { get; set; } = default!;

    [BsonElement("price")]
    public decimal Price { get; set; } = default!;

    [BsonElement("stock")]
    public int Stock { get; set; } = default!;

    [BsonElement("sold")]
    public int Sold { get; set; } = default!;

    [BsonElement("version")]
    public ulong Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
}

public class ProductShortInfoProjection
{
    public static ProductShortInfo Handle(EventEnvelope<ProductRegistered> eventEnvelope)
    {
        var (id, sku, name, desc, status) = eventEnvelope.Data;

        return new ProductShortInfo
        {
            Id = id,
            Sku = sku,
            Name = name,
            Description = desc,
            Status = status,
            Version = eventEnvelope.Metadata.StreamPosition,
            Position = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<ProductModified> eventEnvelope, ProductShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, sku, name, desc) = eventEnvelope.Data;

        view.Sku = sku;
        view.Name = name;
        view.Description = desc;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ProductPriceChanged> eventEnvelope, ProductShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, currency, price) = eventEnvelope.Data;

        view.Currency = currency;
        view.Price = price;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ProductStockChanged> eventEnvelope, ProductShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, stock) = eventEnvelope.Data;

        view.Stock = stock;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ProductSold> eventEnvelope, ProductShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, quantity) = eventEnvelope.Data;

        view.Sold += quantity;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ProductRemoved> eventEnvelope, ProductShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = ProductStatus.Discontinue;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }
}