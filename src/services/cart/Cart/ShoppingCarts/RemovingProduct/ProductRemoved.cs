namespace Cart.ShoppingCarts.RemovingProduct;

public record ProductRemoved(
    Guid CartId,
    ShoppingCartProduct Product
)
{
    public static ProductRemoved Create(Guid cartId, ShoppingCartProduct product)
    {
        if (cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        return new ProductRemoved(cartId, product);
    }
}
