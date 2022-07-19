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
    public ulong LastProcessedPosition { get; set; } = default!;

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
        Description = $"Opened Cart with id {@event.CartId}";
    }

    public void Apply(ProductCartAdded @event)
    {
        AggregateId = @event.CartId;
        Description = $"Added Product with id {@event.Product.ProductId} to Cart with id {@event.CartId}";
    }

    public void Apply(ProductCartRemoved @event)
    {
        AggregateId = @event.CartId;
        Description = $"Removed Product with id {@event.Product.ProductId} from Cart with id {@event.CartId}";
    }

    public void Apply(ShoppingCartConfirmed @event)
    {
        AggregateId = @event.CartId;
        Description = $"Confirmed Cart with id {@event.CartId}";
    }

    public void Apply(ShoppingCartCanceled @event)
    {
        AggregateId = @event.CartId;
        Description = $"Canceled Cart with id {@event.CartId}";
    }
}