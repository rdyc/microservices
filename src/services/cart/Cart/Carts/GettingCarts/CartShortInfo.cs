using Cart.Carts.AddingProduct;
using Cart.Carts.CancelingCart;
using Cart.Carts.ConfirmingCart;
using Cart.Carts.OpeningCart;
using Cart.Carts.RemovingProduct;
using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Cart.Carts.GettingCarts;

[BsonCollection("cart_shortinfo")]
public record CartShortInfo : Document
{
    [BsonElement("client_id")]
    public Guid ClientId { get; set; }

    [BsonElement("total_items_count")]
    public int TotalItemsCount { get; set; }

    [BsonElement("status")]
    public CartStatus Status { get; set; }

    [BsonElement("confirmed_at")]
    public DateTime? ConfirmedAt { get; set; }

    [BsonElement("canceled_at")]
    public DateTime? CanceledAt { get; set; }

    [BsonElement("version")]
    public ulong Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
}

public class CartShortInfoProjection
{
    public static CartShortInfo Handle(EventEnvelope<CartOpened> eventEnvelope)
    {
        var (cartId, clientId, status) = eventEnvelope.Data;

        return new CartShortInfo
        {
            Id = cartId,
            ClientId = clientId,
            Status = status,
            Version = eventEnvelope.Metadata.StreamPosition,
            Position = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<CartProductAdded> eventEnvelope, CartShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.TotalItemsCount += eventEnvelope.Data.Product.Quantity;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<CartProductRemoved> eventEnvelope, CartShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.TotalItemsCount -= eventEnvelope.Data.Product.Quantity;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<CartConfirmed> eventEnvelope, CartShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = CartStatus.Confirmed;
        view.ConfirmedAt = eventEnvelope.Data.ConfirmedAt;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<CartCanceled> eventEnvelope, CartShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = CartStatus.Canceled;
        view.CanceledAt = eventEnvelope.Data.CanceledAt;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }
}