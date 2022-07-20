using Cart.ShoppingCarts.AddingProduct;
using Cart.ShoppingCarts.CancelingCart;
using Cart.ShoppingCarts.ConfirmingCart;
using Cart.ShoppingCarts.OpeningCart;
using Cart.ShoppingCarts.RemovingProduct;
using FW.Core.MongoDB;
using FW.Core.Projections;
using MongoDB.Bson.Serialization.Attributes;

namespace Cart.ShoppingCarts.GettingCartHistory;

[BsonCollection("shopping_cart_history")]
public record ShoppingCartHistory : Document, IVersionedProjection
{
    [BsonElement("aggregate_id")]
    public Guid AggregateId { get; set; } = default!;

    [BsonElement("description")]
    public string Description { get; set; } = default!;

    [BsonElement("last_position")]
    public ulong Position { get; set; } = default!;

    public void When(object @event)
    {
        switch (@event)
        {
            case ShoppingCartOpened cartOpened:
                Apply(cartOpened);
                return;
            case ProductCartAdded cartOpened:
                Apply(cartOpened);
                return;
            case ProductCartRemoved cartOpened:
                Apply(cartOpened);
                return;
            case ShoppingCartConfirmed cartOpened:
                Apply(cartOpened);
                return;
            case ShoppingCartCanceled cartCanceled:
                Apply(cartCanceled);
                return;
        }
    }

    public void Apply(ShoppingCartOpened @event)
    {
        AggregateId = @event.CartId;
        Description = $"Opened";
    }

    public void Apply(ProductCartAdded @event)
    {
        var (id, sku, name, quantity, currency, price) = @event.Product;

        AggregateId = @event.CartId;
        Description = $"Added product for id: {id}, sku: {sku}, name: {name}, quantity: {quantity} and price: {currency.Symbol} {price}";
    }

    public void Apply(ProductCartRemoved @event)
    {
        var (id, sku, name, quantity, currency, price) = @event.Product;

        AggregateId = @event.CartId;
        Description = $"Removed product for id: {id}, sku: {sku}, name: {name}, quantity: {quantity} and price: {currency.Symbol} {price}";
    }

    public void Apply(ShoppingCartConfirmed @event)
    {
        AggregateId = @event.CartId;
        Description = $"Confirmed at {@event.ConfirmedAt}";
    }

    public void Apply(ShoppingCartCanceled @event)
    {
        AggregateId = @event.CartId;
        Description = $"Canceled at {@event.CanceledAt}";
    }
}