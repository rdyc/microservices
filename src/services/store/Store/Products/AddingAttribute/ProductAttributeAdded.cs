using FW.Core.Events;

namespace Store.Products.AddingAttribute;

public record ProductAttributeAdded(
    Guid Id,
    ProductAttribute Attribute
) : IExternalEvent
{
    public static ProductAttributeAdded Create(Guid productId, ProductAttribute productAttribute)
    {
        var (id, name, type, unit, value) = productAttribute;

        return new(productId, ProductAttribute.Create(id, name, type, unit, value));
    }
}