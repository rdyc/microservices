using FW.Core.Events;

namespace Store.Products.RemovingAttribute;

public record ProductAttributeRemoved(
    Guid Id,
    ProductAttribute Attribute
) : IExternalEvent
{
    public static ProductAttributeRemoved Create(Guid productId, ProductAttribute productAttribute)
    {
        var (id, name, type, unit, value) = productAttribute;

        return new(productId, ProductAttribute.Create(id, name, type, unit, value));
    }
}