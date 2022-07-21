using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using Store.Products.AddingAttribute;
using Store.Products.ModifyingProduct;
using Store.Products.RegisteringProduct;
using Store.Products.RemovingAttribute;
using Store.Products.RemovingProduct;
using Store.Products.SellingProduct;
using Store.Products.UpdatingPrice;
using Store.Products.UpdatingStock;

namespace Store.Products.GettingProductHistory;

[BsonCollection("product_history")]
public record ProductHistory : Document
{
    [BsonElement("aggregate_id")]
    public Guid AggregateId { get; set; } = default!;

    [BsonElement("description")]
    public string Description { get; set; } = default!;

    [BsonElement("version")]
    public ulong Version { get; set; } = default!;

    [BsonElement("position")]
    public ulong Position { get; set; } = default!;

    public static ProductHistory Create(Guid aggregateId, string description, EventMetadata metadata)
    {
        var (eventId, streamPosition, logPosition, _) = metadata;

        return new ProductHistory
        {
            Id = Guid.Parse(eventId),
            AggregateId = aggregateId,
            Description = description,
            Version = streamPosition,
            Position = logPosition
        };
    }
}

public class ProductHistoryProjection
{
    public static ProductHistory Handle(EventEnvelope<ProductRegistered> eventEnvelope)
    {
        var (id, sku, name, description, status) = eventEnvelope.Data;

        return ProductHistory.Create(
            id,
            $"Registered as sku: {sku}, name: {name}, description: {description} and status: {status}",
            eventEnvelope.Metadata
        );
    }

    public static ProductHistory Handle(EventEnvelope<ProductModified> eventEnvelope)
    {
        var (id, sku, name, description) = eventEnvelope.Data;

        return ProductHistory.Create(
            id,
            $"Modified as sku: {sku}, name: {name}, and description: {description}",
            eventEnvelope.Metadata
        );
    }

    public static ProductHistory Handle(EventEnvelope<ProductAttributeAdded> eventEnvelope)
    {
        var (id, _, name, type, unit, value) = eventEnvelope.Data;

        return ProductHistory.Create(
            id,
            $"Added attribute with name: {name}, type: {type}, unit: {unit} and value: {value}",
            eventEnvelope.Metadata
        );
    }

    public static ProductHistory Handle(EventEnvelope<ProductAttributeRemoved> eventEnvelope)
    {
        var (id, _, name, type, unit, value) = eventEnvelope.Data;

        return ProductHistory.Create(
            id,
            $"Removed attribute for name: {name}, type: {type}, unit: {unit} and value: {value}",
            eventEnvelope.Metadata
        );
    }

    public static ProductHistory Handle(EventEnvelope<ProductPriceChanged> eventEnvelope)
    {
        var (id, currency, price) = eventEnvelope.Data;

        return ProductHistory.Create(
            id,
            $"Price changed to currency: {currency.Code} and price: {currency.Symbol} {price}",
            eventEnvelope.Metadata
        );
    }

    public static ProductHistory Handle(EventEnvelope<ProductStockChanged> eventEnvelope)
    {
        var (id, stock) = eventEnvelope.Data;

        return ProductHistory.Create(id, $"Stock changed to {stock}", eventEnvelope.Metadata);
    }

    public static ProductHistory Handle(EventEnvelope<ProductSold> eventEnvelope)
    {
        var (id, qty) = eventEnvelope.Data;

        return ProductHistory.Create(id, $"Sold for {qty} item(s)", eventEnvelope.Metadata);
    }

    public static ProductHistory Handle(EventEnvelope<ProductRemoved> eventEnvelope) =>
        ProductHistory.Create(eventEnvelope.Data.Id, "Removed", eventEnvelope.Metadata);
}