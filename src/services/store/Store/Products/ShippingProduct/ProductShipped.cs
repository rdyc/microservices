using FW.Core.Events;

namespace Store.Products.ShippingProduct;

public record ProductShipped(
    Guid Id,
    int Quantity
) : IExternalEvent
{
    public static ProductShipped Create(Guid id, int quantity) =>
        new(id, quantity);
}