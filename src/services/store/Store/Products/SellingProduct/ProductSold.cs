namespace Store.Products.SellingProduct;

public record ProductSold(
    Guid ProductId,
    int Quantity
)
{
    public static ProductSold Create(Guid productId, int quantity) =>
        new(productId, quantity);
}