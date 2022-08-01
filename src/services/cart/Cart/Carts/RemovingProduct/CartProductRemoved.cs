namespace Cart.Carts.RemovingProduct;

public record CartProductRemoved(
    Guid Id,
    CartProduct Product
)
{
    public static CartProductRemoved Create(Guid id, CartProduct product)
    {
        if (id == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(id));

        return new(id, product);
    }
}