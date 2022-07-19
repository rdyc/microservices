using Store.Lookup.Attributes;

namespace Store.Products.AddingAttribute;

public record ProductAttributeAdded(
    Guid Id,
    Guid AttributeId,
    string Name,
    AttributeType Type,
    string Unit,
    string Value
)
{
    public static ProductAttributeAdded Create(Guid productId, ProductAttribute productAttribute)
    {
        var (id, name, type, unit, value) = productAttribute;

        return new(productId, id, name, type, unit, value);
    }
}