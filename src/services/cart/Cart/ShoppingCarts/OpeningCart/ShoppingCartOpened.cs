namespace Cart.ShoppingCarts.OpeningCart;

public record ShoppingCartOpened(
    Guid CartId,
    Guid ClientId,
    ShoppingCartStatus ShoppingCartStatus
)
{
    public static ShoppingCartOpened Create(Guid cartId, Guid clientId, ShoppingCartStatus status)
    {
        if (cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));
        if (clientId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(clientId));
        if (status == default)
            throw new ArgumentOutOfRangeException(nameof(status));

        return new(cartId, clientId, status);
    }
}