namespace Cart.Carts.ConfirmingCart;

public record CartConfirmed(
    Guid CartId,
    DateTime ConfirmedAt
)
{
    public static CartConfirmed Create(Guid cartId, DateTime confirmedAt)
    {
        if (cartId == Guid.Empty)
            throw new ArgumentNullException(nameof(cartId));
        if (confirmedAt == default)
            throw new ArgumentNullException(nameof(confirmedAt));

        return new(cartId, confirmedAt);
    }
}