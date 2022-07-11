namespace Store.Products.RemovingAttribute;

public record AttributeRemoved(
    Guid AttributeId
)
{
    public static AttributeRemoved Create(Guid attributeId)
        => new(attributeId);
}