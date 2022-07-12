namespace Store.Products.UpdatingStock;

public record StockChanged(
    Guid ProductId,
    int Stock
)
{
    public static StockChanged Create(Guid productId, int stock)
        => new(productId, stock);
}