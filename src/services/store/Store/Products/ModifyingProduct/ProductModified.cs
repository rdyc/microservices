namespace Store.Products.ModifyingProduct;

public record ProductModified(
    Guid ProductId,
    string Sku,
    string Name,
    string Description)
{
    public static ProductModified Create(Guid productId, string sku, string name, string description)
    {
        return new ProductModified(productId, sku, name, description);
    }
}