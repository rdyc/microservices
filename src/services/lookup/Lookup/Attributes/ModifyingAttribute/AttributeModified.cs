using FW.Core.Events;

namespace Lookup.Attributes.ModifyingAttribute;

public record AttributeModified(
    Guid AttributeId,
    string Name,
    AttributeType Type,
    string Unit
) : IExternalEvent
{
    public static AttributeModified Create(
        Guid attributeId,
        string name,
        AttributeType type,
        string unit
    ) => new(attributeId, name, type, unit);
}