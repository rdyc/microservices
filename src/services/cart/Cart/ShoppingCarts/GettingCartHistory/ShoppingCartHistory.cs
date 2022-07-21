using Cart.ShoppingCarts.AddingProduct;
using Cart.ShoppingCarts.CancelingCart;
using Cart.ShoppingCarts.ConfirmingCart;
using Cart.ShoppingCarts.OpeningCart;
using Cart.ShoppingCarts.RemovingProduct;
using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Cart.ShoppingCarts.GettingCartHistory;

[BsonCollection("shopping_cart_history")]
public record ShoppingCartHistory : Document
{
    [BsonElement("aggregate_id")]
    public Guid AggregateId { get; set; } = default!;

    [BsonElement("description")]
    public string Description { get; set; } = default!;

    [BsonElement("version")]
    public ulong Version { get; set; } = default!;

    [BsonElement("position")]
    public ulong Position { get; set; } = default!;

    public static ShoppingCartHistory Create(Guid aggregateId, string description, EventMetadata metadata)
    {
        var (eventId, streamPosition, logPosition, _) = metadata;

        return new ShoppingCartHistory
        {
            Id = Guid.Parse(eventId),
            AggregateId = aggregateId,
            Description = description,
            Version = streamPosition,
            Position = logPosition
        };
    }
}

public class ShoppingCartHistoryProjection
{
    public static ShoppingCartHistory Handle(EventEnvelope<ShoppingCartOpened> eventEnvelope)
    {
        var (cartId, clientId, status) = eventEnvelope.Data;

        return ShoppingCartHistory.Create(
            cartId,
            $"Opened as clientId: {clientId} and status: {status}",
            eventEnvelope.Metadata
        );
    }

    public static ShoppingCartHistory Handle(EventEnvelope<ProductCartAdded> eventEnvelope)
    {
        var (id, sku, name, quantity, currency, price) = eventEnvelope.Data.Product;

        return ShoppingCartHistory.Create(
            eventEnvelope.Data.CartId,
            $"Added product for id: {id}, sku: {sku}, name: {name}, quantity: {quantity} and price: {currency.Symbol} {price}",
            eventEnvelope.Metadata
        );
    }

    public static ShoppingCartHistory Handle(EventEnvelope<ProductCartRemoved> eventEnvelope)
    {
        var (id, sku, name, quantity, currency, price) = eventEnvelope.Data.Product;

        return ShoppingCartHistory.Create(
            eventEnvelope.Data.CartId,
            $"Removed product for id: {id}, sku: {sku}, name: {name}, quantity: {quantity} and price: {currency.Symbol} {price}",
            eventEnvelope.Metadata
        );
    }

    public static ShoppingCartHistory Handle(EventEnvelope<ShoppingCartConfirmed> eventEnvelope) =>
        ShoppingCartHistory.Create(
            eventEnvelope.Data.CartId,
            $"Confirmed at {eventEnvelope.Data.ConfirmedAt}",
            eventEnvelope.Metadata
        );

    public static ShoppingCartHistory Handle(EventEnvelope<ShoppingCartCanceled> eventEnvelope) =>
        ShoppingCartHistory.Create(
            eventEnvelope.Data.CartId,
            $"Cancelled at {eventEnvelope.Data.CanceledAt}",
            eventEnvelope.Metadata
        );
}