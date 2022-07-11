namespace Store.Products.UpdatingStock;

public record StockChanged(
    int Stock
)
{
    public static StockChanged Create(int stock)
        => new(stock);
}