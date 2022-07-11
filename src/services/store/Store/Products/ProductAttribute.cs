namespace Store.Products;

public record ProductAttribute
{
    public Guid AttributeId { get; }
    public string Value { get; }

    public ProductAttribute(Guid attributeId, string value)
    {
        AttributeId = attributeId;
        Value = value;
    }

    public static ProductAttribute From(Guid? attributeId, string value)
    {
        if (!attributeId.HasValue)
            throw new ArgumentNullException(nameof(attributeId));

        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value));

        return new ProductAttribute(attributeId.Value, value);
    }

    public bool MatchesAttribute(Guid attributeId)
    {
        return AttributeId == attributeId;
    }

    public bool MatchesAttributeAndValue(ProductAttribute productAttribute)
    {
        return AttributeId == productAttribute.AttributeId && Value == productAttribute.Value;
    }

    public ProductAttribute MergeWith(ProductAttribute productAttribute)
    {
        if (!MatchesAttributeAndValue(productAttribute))
            throw new ArgumentException("Product attribute does not match.");

        return From(productAttribute.AttributeId, productAttribute.Value);
    }

    public bool HasTheSameValue(ProductAttribute productAttribute)
    {
        return Value == productAttribute.Value;
    }
}