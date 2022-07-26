using Order.ShoppingCarts.FinalizingCart;

namespace Order.Orders.InitializingOrder;

public record OrderInitialized(
    Guid OrderId,
    Guid ClientId,
    IEnumerable<ShoppingCartProduct> Products,
    decimal TotalPrice,
    DateTime InitializedAt
)
{
    public static OrderInitialized Create(
        Guid orderId,
        Guid clientId,
        IEnumerable<ShoppingCartProduct> products,
        decimal totalPrice,
        DateTime initializedAt)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (clientId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(clientId));
        if (products is null || !products.Any())
            throw new ArgumentOutOfRangeException(nameof(products));
        if (totalPrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(totalPrice));
        if (initializedAt == default)
            throw new ArgumentOutOfRangeException(nameof(initializedAt));

        return new(orderId, clientId, products, totalPrice, initializedAt);
    }
}