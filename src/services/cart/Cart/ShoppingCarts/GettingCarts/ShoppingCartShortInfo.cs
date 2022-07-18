using Cart.ShoppingCarts.AddingProduct;
using Cart.ShoppingCarts.CancelingCart;
using Cart.ShoppingCarts.ConfirmingCart;
using Cart.ShoppingCarts.OpeningCart;
using Cart.ShoppingCarts.RemovingProduct;
using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Cart.ShoppingCarts.GettingCarts;

[BsonCollection("shopping_cart_shortinfo")]
public record ShoppingCartShortInfo : Document
{
    [BsonElement("client_id")]
    public Guid ClientId { get; set; }

    [BsonElement("total_items_count")]
    public int TotalItemsCount { get; set; }

    [BsonElement("status")]
    public ShoppingCartStatus Status { get; set; }

    [BsonElement("confirmed_at")]
    public DateTime? ConfirmedAt { get; set; }

    [BsonElement("canceled_at")]
    public DateTime? CanceledAt { get; set; }

    [BsonElement("version")]
    public ulong Version { get; set; }

    [BsonElement("position")]
    public ulong LastProcessedPosition { get; set; }
}

public class ShoppingCartShortInfoProjection
{
    public static ShoppingCartShortInfo Handle(EventEnvelope<ShoppingCartOpened> eventEnvelope)
    {
        var (cartId, clientId, status) = eventEnvelope.Data;

        return new ShoppingCartShortInfo
        {
            Id = cartId,
            ClientId = clientId,
            TotalItemsCount = 0,
            Status = status,
            Version = eventEnvelope.Metadata.StreamPosition,
            LastProcessedPosition = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<ProductAdded> eventEnvelope, ShoppingCartShortInfo view)
    {
        if (view.LastProcessedPosition >= eventEnvelope.Metadata.LogPosition)
            return;

        view.TotalItemsCount -= eventEnvelope.Data.Product.Quantity;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.LastProcessedPosition = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ProductRemoved> eventEnvelope, ShoppingCartShortInfo view)
    {
        if (view.LastProcessedPosition >= eventEnvelope.Metadata.LogPosition)
            return;

        view.TotalItemsCount -= eventEnvelope.Data.Product.Quantity;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.LastProcessedPosition = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ShoppingCartConfirmed> eventEnvelope, ShoppingCartShortInfo view)
    {
        if (view.LastProcessedPosition >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = ShoppingCartStatus.Confirmed;
        view.ConfirmedAt = eventEnvelope.Data.ConfirmedAt;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.LastProcessedPosition = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ShoppingCartCanceled> eventEnvelope, ShoppingCartShortInfo view)
    {
        if (view.LastProcessedPosition >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = ShoppingCartStatus.Canceled;
        view.CanceledAt = eventEnvelope.Data.CanceledAt;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.LastProcessedPosition = eventEnvelope.Metadata.LogPosition;
    }
}