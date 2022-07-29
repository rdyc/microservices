using Cart.Carts.AddingProduct;
using Cart.Carts.CancelingCart;
using Cart.Carts.ConfirmingCart;
using Cart.Carts.OpeningCart;
using Cart.Carts.RemovingProduct;
using FW.Core.Aggregates;
using FW.Core.Extensions;

namespace Cart.Carts;

public class Cart : Aggregate
{
    public Cart()
    {
    }

    public Guid ClientId { get; private set; }
    public CartStatus Status { get; private set; }
    public IList<CartProduct> Products { get; private set; } = default!;
    public int TotalItems => Products.Sum(e => e.Quantity);
    public decimal TotalPrice => Products.Sum(e => e.TotalPrice);

    private Cart(Guid id, Guid clientId, CartStatus status)
    {
        var @event = CartOpened.Create(id, clientId, status);

        Enqueue(@event);
        Apply(@event);
    }

    public static Cart Open(Guid cartId, Guid clientId)
        => new(cartId, clientId, CartStatus.Pending);

    public void Apply(CartOpened @event)
    {
        Version = 0;

        Id = @event.CartId;
        ClientId = @event.ClientId;
        Products = new List<CartProduct>();
        Status = CartStatus.Pending;
    }

    public void AddProduct(CartProduct product)
    {
        if (Status != CartStatus.Pending)
            throw new InvalidOperationException($"Adding product for the cart in '{Status}' status is not allowed.");

        var @event = CartProductAdded.Create(Id, product);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(CartProductAdded @event)
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

    public void RemoveProduct(CartProduct productToBeRemoved)
    {
        if (Status != CartStatus.Pending)
            throw new InvalidOperationException($"Removing product from the cart in '{Status}' status is not allowed.");

        var existingProduct = FindProductMatchingWith(productToBeRemoved);

        if (existingProduct is null)
            throw new InvalidOperationException($"Product with id `{productToBeRemoved.ProductId}` and price '{productToBeRemoved.Price}' was not found in cart.");

        if (!existingProduct.HasEnough(productToBeRemoved.Quantity))
            throw new InvalidOperationException($"Cannot remove {productToBeRemoved.Quantity} items of Product with id `{productToBeRemoved.ProductId}` as there are only ${existingProduct.Quantity} items in card");

        var @event = CartProductRemoved.Create(Id, productToBeRemoved);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(CartProductRemoved @event)
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
        if (Status != CartStatus.Pending)
            throw new InvalidOperationException($"Confirming cart in '{Status}' status is not allowed.");

        var @event = CartConfirmed.Create(Id, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(CartConfirmed _)
    {
        Version++;

        Status = CartStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status != CartStatus.Pending)
            throw new InvalidOperationException($"Canceling cart in '{Status}' status is not allowed.");

        var @event = CartCanceled.Create(Id, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(CartCanceled _)
    {
        Version++;

        Status = CartStatus.Canceled;
    }

    public override void When(object @event)
    {
        switch (@event)
        {
            case CartOpened cartOpened:
                Apply(cartOpened);
                return;
            case CartProductAdded productAdded:
                Apply(productAdded);
                return;
            case CartProductRemoved productRemoved:
                Apply(productRemoved);
                return;
            case CartConfirmed cartConfirmed:
                Apply(cartConfirmed);
                return;
            case CartCanceled cartCanceled:
                Apply(cartCanceled);
                return;
        }
    }

    private CartProduct? FindProductMatchingWith(CartProduct product)
        => Products.SingleOrDefault(e => e.MatchesProductAndPrice(product));
}