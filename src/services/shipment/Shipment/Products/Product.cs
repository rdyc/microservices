using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using Shipment.Products.ModifyingProduct;
using Shipment.Products.RegisteringProduct;
using Shipment.Products.RemovingProduct;
using Shipment.Products.ShippingProduct;
using Shipment.Products.UpdatingStock;

namespace Shipment.Products;

[BsonCollection("product")]
public record Product : Document
{
    [BsonElement("sku")]
    public string Sku { get; set; } = default!;

    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("description")]
    public string Description { get; set; } = default!;

    [BsonElement("status")]
    public ProductStatus Status { get; set; } = default!;

    [BsonElement("stock")]
    public int Stock { get; set; } = default!;

    [BsonElement("version")]
    public ulong Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
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

    public static void Handle(EventEnvelope<ProductStockChanged> eventEnvelope, Product view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, stock) = eventEnvelope.Data;

        view.Stock += stock;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ProductShipped> eventEnvelope, Product view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, quantity) = eventEnvelope.Data;

        view.Stock -= quantity;
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