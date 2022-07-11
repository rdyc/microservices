namespace Store.Products.ModifyingProduct;

public record ProductModified(
    string SKU,
    string Name,
    string Description)
{
    public static ProductModified Create(string sku, string name, string description)
    {
        return new ProductModified(sku, name, description);
    }
}