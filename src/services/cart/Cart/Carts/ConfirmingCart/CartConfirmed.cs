namespace Cart.Carts.ConfirmingCart;

public record CartConfirmed(
    Guid Id,
    DateTime ConfirmedAt
)
{
    public static CartConfirmed Create(Guid id, DateTime confirmedAt)
    {
        if (id == Guid.Empty)
            throw new ArgumentNullException(nameof(id));
        if (confirmedAt == default)
            throw new ArgumentNullException(nameof(confirmedAt));

        return new(id, confirmedAt);
    }
}