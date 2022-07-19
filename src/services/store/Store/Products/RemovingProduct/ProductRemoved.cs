namespace Store.Products.RemovingProduct;

public record ProductRemoved(Guid Id)
{
    public static ProductRemoved Create(Guid id)
        => new(id);
}