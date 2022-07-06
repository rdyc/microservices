namespace Lookup.Attributes.ModifyingAttribute;

public record AttributeModified(Guid Id, string Name, AttributeType Type, string Unit)
{
    public static AttributeModified Create(Guid id, string name, AttributeType type, string unit)
    {
        return new AttributeModified(id, name, type, unit);
    }
}