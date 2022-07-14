using Cart.ShoppingCarts.AddingProduct;
using Cart.ShoppingCarts.CancelingCart;
using Cart.ShoppingCarts.ConfirmingCart;
using Cart.ShoppingCarts.OpeningCart;
using Cart.ShoppingCarts.RemovingProduct;
using FW.Core.Extensions;
using FW.Core.Projections;

namespace Cart.ShoppingCarts.GettingCartById;

public class ShoppingCartDetails : IVersionedProjection
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public ShoppingCartStatus Status { get; set; }
    public IList<ShoppingCartProduct> Products { get; set; } = default!;
    public decimal TotalPrice => Products.Sum(pi => pi.TotalPrice);
    public long Version { get; set; }


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
        ClientId = @event.ClientId;
        Products = new List<ShoppingCartProduct>();
        Status = @event.ShoppingCartStatus;
        Version = 0;
    }

    public void Apply(ProductAdded @event)
    {
        Version++;

        var newProduct = @event.Product;

        var existinwProduct = FindProductMatchingWith(newProduct);

        if (existinwProduct is null)
        {
            Products.Add(newProduct);
            return;
        }

        Products.Replace(
            existinwProduct,
            existinwProduct.MergeWith(newProduct)
        );
    }

    public void Apply(ProductRemoved @event)
    {
        Version++;

        var productItemToBeRemoved = @event.Product;

        var existinwProduct = FindProductMatchingWith(@event.Product);

        if (existinwProduct == null)
            return;

        if (existinwProduct.HasTheSameQuantity(productItemToBeRemoved))
        {
            Products.Remove(existinwProduct);
            return;
        }

        Products.Replace(
            existinwProduct,
            existinwProduct.Substract(productItemToBeRemoved)
        );
    }

    public void Apply(ShoppingCartConfirmed @event)
    {
        Version++;

        Status = ShoppingCartStatus.Confirmed;
    }

    public void Apply(ShoppingCartCanceled @event)
    {
        Version++;

        Status = ShoppingCartStatus.Canceled;
    }

    private ShoppingCartProduct? FindProductMatchingWith(ShoppingCartProduct product)
        => Products.SingleOrDefault(e => e.MatchesProductAndPrice(product));

    public ulong LastProcessedPosition { get; set; }
}
