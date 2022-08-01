using Cart.Carts.AddingProduct;
using Cart.Carts.CancelingCart;
using Cart.Carts.ConfirmingCart;
using Cart.Carts.OpeningCart;
using Cart.Carts.RemovingProduct;
using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Cart.Carts.GettingCartHistory;

[BsonCollection("cart_history")]
public record CartHistory : Document
{
    [BsonElement("aggregate_id")]
    public Guid AggregateId { get; set; } = default!;

    [BsonElement("description")]
    public string Description { get; set; } = default!;

    [BsonElement("version")]
    public ulong Version { get; set; } = default!;

    [BsonElement("position")]
    public ulong Position { get; set; } = default!;

    public static CartHistory Create(Guid aggregateId, string description, EventMetadata metadata)
    {
        var (eventId, streamPosition, logPosition, _) = metadata;

        return new CartHistory
        {
            Id = Guid.Parse(eventId),
            AggregateId = aggregateId,
            Description = description,
            Version = streamPosition,
            Position = logPosition
        };
    }
}

public class CartHistoryProjection
{
    public static CartHistory Handle(EventEnvelope<CartOpened> eventEnvelope)
    {
        var (id, clientId, status) = eventEnvelope.Data;

        return CartHistory.Create(
            id,
            $"Opened as clientId: {clientId} and status: {status}",
            eventEnvelope.Metadata
        );
    }

    public static CartHistory Handle(EventEnvelope<CartProductAdded> eventEnvelope)
    {
        var (_, sku, name, quantity, currency, price) = eventEnvelope.Data.Product;

        return CartHistory.Create(
            eventEnvelope.Data.Id,
            $"Added product for sku: {sku}, name: {name}, quantity: {quantity} and price: {currency.Symbol} {price}",
            eventEnvelope.Metadata
        );
    }

    public static CartHistory Handle(EventEnvelope<CartProductRemoved> eventEnvelope)
    {
        var (_, sku, name, quantity, currency, price) = eventEnvelope.Data.Product;

        return CartHistory.Create(
            eventEnvelope.Data.Id,
            $"Removed product for sku: {sku}, name: {name}, quantity: {quantity} and price: {currency.Symbol} {price}",
            eventEnvelope.Metadata
        );
    }

    public static CartHistory Handle(EventEnvelope<CartConfirmed> eventEnvelope) =>
        CartHistory.Create(
            eventEnvelope.Data.Id,
            $"Confirmed at {eventEnvelope.Data.ConfirmedAt}",
            eventEnvelope.Metadata
        );

    public static CartHistory Handle(EventEnvelope<CartCanceled> eventEnvelope) =>
        CartHistory.Create(
            eventEnvelope.Data.Id,
            $"Cancelled at {eventEnvelope.Data.CanceledAt}",
            eventEnvelope.Metadata
        );
}