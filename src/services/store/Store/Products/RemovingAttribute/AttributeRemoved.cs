using Store.Attributes;

namespace Store.Products.RemovingAttribute;

public record AttributeRemoved(
    Guid ProductId,
    Guid AttributeId,
    string Name,
    AttributeType Type,
    string Unit,
    string Value
)
{
    public static AttributeRemoved Create(Guid productId, ProductAttribute productAttribute)
    {
        var (id, name, type, unit, value) = productAttribute;

        return new(productId, id, name, type, unit, value);
    }
}