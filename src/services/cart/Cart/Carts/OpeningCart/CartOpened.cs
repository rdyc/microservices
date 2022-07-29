namespace Cart.Carts.OpeningCart;

public record CartOpened(
    Guid CartId,
    Guid ClientId,
    CartStatus Status
)
{
    public static CartOpened Create(Guid cartId, Guid clientId, CartStatus status)
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