namespace Cart.Carts.RemovingProduct;

public record CartProductRemoved(
    Guid CartId,
    CartProduct Product
)
{
    public static CartProductRemoved Create(Guid cartId, CartProduct product)
    {
        if (cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        return new(cartId, product);
    }
}