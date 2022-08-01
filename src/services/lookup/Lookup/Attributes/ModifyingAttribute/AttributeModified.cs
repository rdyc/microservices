using FW.Core.Events;

namespace Lookup.Attributes.ModifyingAttribute;

public record AttributeModified(
    Guid Id,
    string Name,
    AttributeType Type,
    string Unit
) : IExternalEvent
{
    public static AttributeModified Create(
        Guid id,
        string name,
        AttributeType type,
        string unit
    ) => new(id, name, type, unit);
}