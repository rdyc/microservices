namespace Cart.ShoppingCarts.AddingProduct;

public record ProductCartAdded(
    Guid CartId,
    ShoppingCartProduct Product
)
{
    public static ProductCartAdded Create(Guid cartId, ShoppingCartProduct product)
    {
        if (cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        return new ProductCartAdded(cartId, product);
    }
}