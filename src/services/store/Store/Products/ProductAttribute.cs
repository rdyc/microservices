using Attribute = Store.Attributes.Attribute;

namespace Store.Products;

public class ProductAttribute
{
    public Guid AttributeId => Attribute.Id;
    public Attribute Attribute { get; private set; } = default!;
    public string Value { get; private set; } = default!;

    public ProductAttribute(Attribute attribute, string value)
    {
        Attribute = attribute;
        Value = value;
    }

    public static ProductAttribute From(Guid? attributeId, Attribute attribute, string value)
    {
        if (!attributeId.HasValue)
            throw new ArgumentNullException(nameof(attributeId));

        if (attribute is null)
            throw new ArgumentNullException(nameof(attribute));

        return value switch
        {
            null => throw new ArgumentNullException(nameof(value)),
            // <= 0 => throw new ArgumentOutOfRangeException(nameof(value), "Value has to be a positive number"),
            _ => new ProductAttribute(attribute, value)
        };
    }

    public bool MatchesAttribute(ProductAttribute productAttribute)
    {
        return AttributeId == productAttribute.AttributeId;
    }

    public ProductAttribute MergeWith(ProductAttribute productAttribute)
    {
        if (!MatchesAttribute(productAttribute))
            throw new ArgumentException("Product attribute does not match.");

        return From(AttributeId, productAttribute.Attribute, productAttribute.Value);
    }

    public ProductAttribute Subtract(ProductAttribute productAttribute)
    {
        if (!MatchesAttribute(productAttribute))
            throw new ArgumentException("Product attribute not match.");

        return From(AttributeId, productAttribute.Attribute, productAttribute.Value);
    }

    public bool HasTheSameValue(ProductAttribute productAttribute)
    {
        return Value == productAttribute.Value;
    }
}