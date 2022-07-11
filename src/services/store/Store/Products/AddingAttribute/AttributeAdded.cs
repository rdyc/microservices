namespace Store.Products.AddingAttribute;

public record AttributeAdded(
    Guid AttributeId,
    string Value
)
{
    public static AttributeAdded Create(Guid attributeId, string value)
        => new(attributeId, value);
}