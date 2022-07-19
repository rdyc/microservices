namespace Store.Products.SellingProduct;

public record ProductSold(
    Guid Id,
    int Quantity
)
{
    public static ProductSold Create(Guid id, int quantity) =>
        new(id, quantity);
}