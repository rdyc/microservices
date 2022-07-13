using Store.Lookup.Attributes;

namespace Store.Products;

public record ProductAttribute
(
    Guid Id,
    string Name,
    AttributeType Type,
    string Unit,
    string Value
)
{
    public static ProductAttribute From(Guid? id, string name, AttributeType type, string unit, string value)
    {
        if (!id.HasValue)
            throw new ArgumentNullException(nameof(id));

        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        if (string.IsNullOrEmpty(unit))
            throw new ArgumentNullException(nameof(unit));

        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value));

        return new ProductAttribute(id.Value, name, type, unit, value);
    }

    public bool MatchesAttribute(Guid attributeId)
    {
        return Id == attributeId;
    }

    public bool MatchesAttributeAndValue(ProductAttribute attribute)
    {
        return Id == attribute.Id && Value == attribute.Value;
    }

    public ProductAttribute MergeWith(ProductAttribute attribute)
    {
        if (!MatchesAttributeAndValue(attribute))
            throw new ArgumentException("Product attribute does not match.");

        var (id, name, type, unit, value) = attribute;

        return From(id, name, type, unit, value);
    }

    public bool HasTheSameValue(ProductAttribute attribute)
    {
        return Value == attribute.Value;
    }
}