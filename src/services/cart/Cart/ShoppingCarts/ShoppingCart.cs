using Cart.ShoppingCarts.AddingProduct;
using Cart.ShoppingCarts.CancelingCart;
using Cart.ShoppingCarts.ConfirmingCart;
using Cart.ShoppingCarts.OpeningCart;
using Cart.ShoppingCarts.RemovingProduct;
using FW.Core.Aggregates;
using FW.Core.Extensions;

namespace Cart.ShoppingCarts;

public class ShoppingCart : Aggregate
{
    public ShoppingCart()
    {
    }

    public Guid ClientId { get; private set; }
    public ShoppingCartStatus Status { get; private set; }
    public IList<ShoppingCartProduct> Products { get; private set; } = default!;
    public int TotalItems => Products.Sum(e => e.Quantity);
    public decimal TotalPrice => Products.Sum(e => e.TotalPrice);

    private ShoppingCart(Guid id, Guid clientId, ShoppingCartStatus status)
    {
        var @event = ShoppingCartOpened.Create(id, clientId, status);

        Enqueue(@event);
        Apply(@event);
    }

    public override void When(object @event)
    {
        switch (@event)
        {
            case ShoppingCartOpened cartOpened:
                Apply(cartOpened);
                return;
            case ProductCartAdded productAdded:
                Apply(productAdded);
                return;
            case ProductCartRemoved productRemoved:
                Apply(productRemoved);
                return;
            case ShoppingCartConfirmed cartConfirmed:
                Apply(cartConfirmed);
                return;
            case ShoppingCartCanceled cartCanceled:
                Apply(cartCanceled);
                return;
        }
    }

    public static ShoppingCart Open(Guid cartId, Guid clientId)
        => new(cartId, clientId, ShoppingCartStatus.Pending);

    public void Apply(ShoppingCartOpened @event)
    {
        Version = 0;

        Id = @event.CartId;
        ClientId = @event.ClientId;
        Products = new List<ShoppingCartProduct>();
        Status = ShoppingCartStatus.Pending;
    }

    public void AddProduct(ShoppingCartProduct product)
    {
        if (Status != ShoppingCartStatus.Pending)
            throw new InvalidOperationException($"Adding product for the cart in '{Status}' status is not allowed.");

        var @event = ProductCartAdded.Create(Id, product);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(ProductCartAdded @event)
    {
        Version++;

        var newProductItem = @event.Product;

        var existingProductItem = FindProductMatchingWith(newProductItem);

        if (existingProductItem is null)
        {
            Products.Add(newProductItem);
            return;
        }

        Products.Replace(
            existingProductItem,
            existingProductItem.MergeWith(newProductItem)
        );
    }

    public void RemoveProduct(ShoppingCartProduct productToBeRemoved)
    {
        if (Status != ShoppingCartStatus.Pending)
            throw new InvalidOperationException($"Removing product from the cart in '{Status}' status is not allowed.");

        var existingProduct = FindProductMatchingWith(productToBeRemoved);

        if (existingProduct is null)
            throw new InvalidOperationException($"Product with id `{productToBeRemoved.ProductId}` and price '{productToBeRemoved.Price}' was not found in cart.");

        if (!existingProduct.HasEnough(productToBeRemoved.Quantity))
            throw new InvalidOperationException($"Cannot remove {productToBeRemoved.Quantity} items of Product with id `{productToBeRemoved.ProductId}` as there are only ${existingProduct.Quantity} items in card");

        var @event = ProductCartRemoved.Create(Id, productToBeRemoved);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(ProductCartRemoved @event)
    {
        Version++;

        var productToBeRemoved = @event.Product;

        var existingProduct = FindProductMatchingWith(@event.Product);

        if (existingProduct == null)
            return;

        if (existingProduct.HasTheSameQuantity(productToBeRemoved))
        {
            Products.Remove(existingProduct);
            return;
        }

        Products.Replace(
            existingProduct,
            existingProduct.Substract(productToBeRemoved)
        );
    }

    public void Confirm()
    {
        if (Status != ShoppingCartStatus.Pending)
            throw new InvalidOperationException($"Confirming cart in '{Status}' status is not allowed.");

        var @event = ShoppingCartConfirmed.Create(Id, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(ShoppingCartConfirmed @event)
    {
        Version++;

        Status = ShoppingCartStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status != ShoppingCartStatus.Pending)
            throw new InvalidOperationException($"Canceling cart in '{Status}' status is not allowed.");

        var @event = ShoppingCartCanceled.Create(Id, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(ShoppingCartCanceled @event)
    {
        Version++;

        Status = ShoppingCartStatus.Canceled;
    }

    private ShoppingCartProduct? FindProductMatchingWith(ShoppingCartProduct product)
        => Products.SingleOrDefault(e => e.MatchesProductAndPrice(product));
}