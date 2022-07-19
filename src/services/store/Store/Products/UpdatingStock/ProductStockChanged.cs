namespace Store.Products.UpdatingStock;

public record ProductStockChanged(
    Guid Id,
    int Stock
)
{
    public static ProductStockChanged Create(Guid id, int stock)
        => new(id, stock);
}