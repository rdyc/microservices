using FW.Core.Events;
using Store.Lookup.Attributes;

namespace Store.Products.AddingAttribute;

public record ProductAttributeAdded(
    Guid ProductId,
    Guid AttributeId,
    string Name,
    AttributeType Type,
    string Unit,
    string Value
) : IExternalEvent
{
    public static ProductAttributeAdded Create(Guid productId, ProductAttribute productAttribute)
    {
        var (id, name, type, unit, value) = productAttribute;

        return new(productId, id, name, type, unit, value);
    }
}