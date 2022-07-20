using FW.Core.Events;

namespace Store.Products.RemovingProduct;

public record ProductRemoved(
    Guid Id
) : IExternalEvent
{
    public static ProductRemoved Create(Guid id) =>
        new(id);
}