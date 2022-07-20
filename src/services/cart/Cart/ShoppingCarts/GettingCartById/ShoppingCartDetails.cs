using Cart.ShoppingCarts.AddingProduct;
using Cart.ShoppingCarts.CancelingCart;
using Cart.ShoppingCarts.ConfirmingCart;
using Cart.ShoppingCarts.OpeningCart;
using Cart.ShoppingCarts.RemovingProduct;
using FW.Core.Events;
using FW.Core.Extensions;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Cart.ShoppingCarts.GettingCartById;

[BsonCollection("shopping_cart_details")]
public record ShoppingCartDetails : Document
{
    [BsonElement("client_id")]
    public Guid ClientId { get; set; }

    [BsonElement("status")]
    public ShoppingCartStatus Status { get; set; }

    [BsonElement("products")]
    public IList<ShoppingCartProduct> Products { get; set; } = default!;

    [BsonElement("total_price")]
    public decimal TotalPrice => Products.Sum(pi => pi.TotalPrice);

    [BsonElement("confirmed_at")]
    public DateTime? ConfirmedAt { get; set; }

    [BsonElement("canceled_at")]
    public DateTime? CanceledAt { get; set; }

    [BsonElement("version")]
    public ulong Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
}

public class ShoppingCartDetailsProjection
{
    public static ShoppingCartDetails Handle(EventEnvelope<ShoppingCartOpened> eventEnvelope)
    {
        var (cartId, clientId, status) = eventEnvelope.Data;

        return new ShoppingCartDetails
        {
            Id = cartId,
            ClientId = clientId,
            Products = new List<ShoppingCartProduct>(),
            Status = status
        };
    }

    public static void Handle(EventEnvelope<ProductCartAdded> eventEnvelope, ShoppingCartDetails view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

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

    public static void Handle(EventEnvelope<ProductCartRemoved> eventEnvelope, ShoppingCartDetails view)
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

    public static void Handle(EventEnvelope<ShoppingCartConfirmed> eventEnvelope, ShoppingCartDetails view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = ShoppingCartStatus.Confirmed;
        view.ConfirmedAt = eventEnvelope.Data.ConfirmedAt;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ShoppingCartCanceled> eventEnvelope, ShoppingCartDetails view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = ShoppingCartStatus.Canceled;
        view.CanceledAt = eventEnvelope.Data.CanceledAt;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    private static ShoppingCartProduct? FindProductMatchingWith(IEnumerable<ShoppingCartProduct> products, ShoppingCartProduct product)
        => products.SingleOrDefault(e => e.MatchesProductAndPrice(product));
}