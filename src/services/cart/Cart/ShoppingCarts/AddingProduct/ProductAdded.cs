namespace Cart.ShoppingCarts.AddingProduct;

public record ProductAdded(
    Guid CartId,
    ShoppingCartProduct Product
)
{
    public static ProductAdded Create(Guid cartId, ShoppingCartProduct product)
    {
        if (cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        return new ProductAdded(cartId, product);
    }
}
