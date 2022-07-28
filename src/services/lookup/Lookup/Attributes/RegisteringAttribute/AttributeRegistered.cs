using FW.Core.Events;

namespace Lookup.Attributes.RegisteringAttribute;

public record AttributeRegistered(
    Guid AttributeId,
    string Name,
    AttributeType Type,
    string Unit,
    LookupStatus Status
) : IExternalEvent
{
    public static AttributeRegistered Create(
        Guid attributeId,
        string name,
        AttributeType type,
        string unit,
        LookupStatus status
    ) => new(attributeId, name, type, unit, status);
}