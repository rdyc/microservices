namespace Cart.ShoppingCarts.RemovingProduct;

public record ProductCartRemoved(
    Guid CartId,
    ShoppingCartProduct Product
)
{
    public static ProductCartRemoved Create(Guid cartId, ShoppingCartProduct product)
    {
        if (cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        return new ProductCartRemoved(cartId, product);
    }
}
