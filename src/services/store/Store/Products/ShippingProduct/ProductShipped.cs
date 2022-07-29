using FW.Core.Events;

namespace Store.Products.ShippingProduct;

public record ProductShipped(
    Guid ProductId,
    int Quantity
) : IExternalEvent
{
    public static ProductShipped Create(Guid productId, int quantity) =>
        new(productId, quantity);
}