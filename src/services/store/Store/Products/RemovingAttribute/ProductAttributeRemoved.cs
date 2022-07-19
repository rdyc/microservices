using Store.Lookup.Attributes;

namespace Store.Products.RemovingAttribute;

public record ProductAttributeRemoved(
    Guid Id,
    Guid AttributeId,
    string Name,
    AttributeType Type,
    string Unit,
    string Value
)
{
    public static ProductAttributeRemoved Create(Guid productId, ProductAttribute productAttribute)
    {
        var (id, name, type, unit, value) = productAttribute;

        return new(productId, id, name, type, unit, value);
    }
}