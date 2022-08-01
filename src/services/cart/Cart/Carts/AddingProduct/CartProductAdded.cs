namespace Cart.Carts.AddingProduct;

public record CartProductAdded(
    Guid Id,
    CartProduct Product
)
{
    public static CartProductAdded Create(Guid id, CartProduct product)
    {
        if (id == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(id));

        return new (id, product);
    }
}