namespace Store.Products.RemovingProduct;

public record ProductRemoved(Guid ProductId)
{
    public static ProductRemoved Create(Guid productId)
    {
        return new ProductRemoved(productId);
    }
}