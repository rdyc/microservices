namespace Lookup.Attributes.RegisteringAttribute;

public record AttributeRegistered(Guid Id, string Name, AttributeType Type, string Unit, LookupStatus Status)
{
    public static AttributeRegistered Create(Guid id, string name, AttributeType type, string unit, LookupStatus status)
    {
        return new AttributeRegistered(id, name, type, unit, status);
    }
}