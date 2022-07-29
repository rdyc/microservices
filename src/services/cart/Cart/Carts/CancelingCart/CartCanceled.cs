namespace Cart.Carts.CancelingCart;

public record CartCanceled(
    Guid CartId,
    DateTime CanceledAt
)
{
    public static CartCanceled Create(Guid cartId, DateTime canceledAt)
    {
        if (cartId == Guid.Empty)
            throw new ArgumentNullException(nameof(cartId));
        if (canceledAt == default)
            throw new ArgumentNullException(nameof(canceledAt));

        return new CartCanceled(cartId, canceledAt);
    }
}