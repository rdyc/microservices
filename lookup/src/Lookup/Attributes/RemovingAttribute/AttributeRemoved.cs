namespace Lookup.Attributes.RemovingAttribute;

public record AttributeRemoved(Guid Id)
{
    public static AttributeRemoved Create(Guid id)
    {
        return new AttributeRemoved(id);
    }
}