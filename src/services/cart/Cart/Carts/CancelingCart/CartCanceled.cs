namespace Cart.Carts.CancelingCart;

public record CartCanceled(
    Guid Id,
    DateTime CanceledAt
)
{
    public static CartCanceled Create(Guid id, DateTime canceledAt)
    {
        if (id == Guid.Empty)
            throw new ArgumentNullException(nameof(id));
        if (canceledAt == default)
            throw new ArgumentNullException(nameof(canceledAt));

        return new CartCanceled(id, canceledAt);
    }
}