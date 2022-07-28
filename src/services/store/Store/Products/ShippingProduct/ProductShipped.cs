namespace Store.Products.ShippingProduct;

public record ProductShipped(
    Guid ProductId,
    int Quantity
)
{
    public static ProductShipped Create(Guid productId, int quantity) =>
        new(productId, quantity);
}