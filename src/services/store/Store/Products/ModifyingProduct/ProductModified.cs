namespace Store.Products.ModifyingProduct;

public record ProductModified(
    Guid Id,
    string Sku,
    string Name,
    string Description)
{
    public static ProductModified Create(Guid id, string sku, string name, string description)
    {
        return new ProductModified(id, sku, name, description);
    }
}