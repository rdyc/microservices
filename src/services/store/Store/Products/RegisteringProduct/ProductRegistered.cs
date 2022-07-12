namespace Store.Products.RegisteringProduct;

public record ProductRegistered(
    Guid ProductId,
    string SKU,
    string Name,
    string Description)
{
    public static ProductRegistered Create(Guid productId, string sku, string name, string description)
    {
        return new ProductRegistered(productId, sku, name, description);
    }
}