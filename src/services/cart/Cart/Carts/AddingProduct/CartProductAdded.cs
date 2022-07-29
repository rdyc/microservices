namespace Cart.Carts.AddingProduct;

public record CartProductAdded(
    Guid CartId,
    CartProduct Product
)
{
    public static CartProductAdded Create(Guid cartId, CartProduct product)
    {
        if (cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        return new (cartId, product);
    }
}