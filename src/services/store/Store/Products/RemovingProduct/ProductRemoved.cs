using FW.Core.Events;

namespace Store.Products.RemovingProduct;

public record ProductRemoved(
    Guid ProductId
) : IExternalEvent
{
    public static ProductRemoved Create(Guid productId) =>
        new(productId);
}