using Cart.ShoppingCarts.CancelingCart;
using Cart.ShoppingCarts.ConfirmingCart;
using Cart.ShoppingCarts.OpeningCart;
using Cart.ShoppingCarts.RemovingProduct;
using Cart.ShoppingCarts.AddingProduct;
using FW.Core.Projections;

namespace Cart.ShoppingCarts.GettingCarts;

public class ShoppingCartShortInfo : IVersionedProjection
{
    public Guid Id { get; set; }

    public int TotalItemsCount { get; set; }

    public ShoppingCartStatus Status { get; set; }

    public ulong LastProcessedPosition { get; set; }

    public void When(object @event)
    {
        switch (@event)
        {
            case ShoppingCartOpened cartOpened:
                Apply(cartOpened);
                return;
            case ProductAdded cartOpened:
                Apply(cartOpened);
                return;
            case ProductRemoved cartOpened:
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
        Id = @event.CartId;
        TotalItemsCount = 0;
        Status = ShoppingCartStatus.Pending;
    }

    public void Apply(ProductAdded @event)
    {
        TotalItemsCount += @event.Product.Quantity;
    }

    public void Apply(ProductRemoved @event)
    {
        TotalItemsCount -= @event.Product.Quantity;
    }

    public void Apply(ShoppingCartConfirmed @event)
    {
        Status = ShoppingCartStatus.Confirmed;
    }

    public void Apply(ShoppingCartCanceled @event)
    {
        Status = ShoppingCartStatus.Canceled;
    }
}
