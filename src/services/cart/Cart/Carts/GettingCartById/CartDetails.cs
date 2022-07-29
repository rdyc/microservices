using Cart.Carts.AddingProduct;
using Cart.Carts.CancelingCart;
using Cart.Carts.ConfirmingCart;
using Cart.Carts.OpeningCart;
using Cart.Carts.RemovingProduct;
using FW.Core.Events;
using FW.Core.Extensions;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Cart.Carts.GettingCartById;

[BsonCollection("cart_details")]
public record CartDetails : Document
{
    [BsonElement("client_id")]
    public Guid ClientId { get; set; }

    [BsonElement("status")]
    public CartStatus Status { get; set; }

    [BsonElement("products")]
    public IList<CartProduct> Products { get; set; } = default!;

    [BsonElement("total_price")]
    public decimal TotalPrice { get; set; } = default!;

    [BsonElement("confirmed_at")]
    public DateTime? ConfirmedAt { get; set; }

    [BsonElement("canceled_at")]
    public DateTime? CanceledAt { get; set; }

    [BsonElement("version")]
    public ulong Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
}

public class CartDetailsProjection
{
    public static CartDetails Handle(EventEnvelope<CartOpened> eventEnvelope)
    {
        var (cartId, clientId, status) = eventEnvelope.Data;

        return new CartDetails
        {
            Id = cartId,
            ClientId = clientId,
            Status = status
        };
    }

    public static void Handle(EventEnvelope<CartProductAdded> eventEnvelope, CartDetails view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        if (view.Products is null)
            view.Products = new List<CartProduct>();

        var newProduct = eventEnvelope.Data.Product;
        var existingProduct = FindProductMatchingWith(view.Products, newProduct);

        if (existingProduct is null)
        {
            view.Products.Add(newProduct);
        }
        else
        {
            view.Products.Replace(
                existingProduct,
                existingProduct.MergeWith(newProduct)
            );
        }
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<CartProductRemoved> eventEnvelope, CartDetails view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var productItemToBeRemoved = eventEnvelope.Data.Product;
        var existingProduct = FindProductMatchingWith(view.Products, eventEnvelope.Data.Product);

        if (existingProduct != null)
        {
            if (existingProduct.HasTheSameQuantity(productItemToBeRemoved))
            {
                view.Products.Remove(existingProduct);
            }
            else
            {
                view.Products.Replace(
                    existingProduct,
                    existingProduct.Substract(productItemToBeRemoved)
                );
            }
        }
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<CartConfirmed> eventEnvelope, CartDetails view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = CartStatus.Confirmed;
        view.ConfirmedAt = eventEnvelope.Data.ConfirmedAt;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<CartCanceled> eventEnvelope, CartDetails view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = CartStatus.Canceled;
        view.CanceledAt = eventEnvelope.Data.CanceledAt;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    private static CartProduct? FindProductMatchingWith(IEnumerable<CartProduct> products, CartProduct product)
        => products.SingleOrDefault(e => e.MatchesProductAndPrice(product));
}