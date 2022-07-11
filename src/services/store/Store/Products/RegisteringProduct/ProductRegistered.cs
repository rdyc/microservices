namespace Store.Products.RegisteringProduct;

public record ProductRegistered(Guid Id, string SKU, string Name, string Description)
{
    public static ProductRegistered Create(Guid id, string sku, string name, string description)
    {
        return new ProductRegistered(id, sku, name, description);
    }
}