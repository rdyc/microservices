using FW.Core.Events;

namespace Lookup.Attributes.RegisteringAttribute;

public record AttributeRegistered(
    Guid Id,
    string Name,
    AttributeType Type,
    string Unit,
    LookupStatus Status
) : IExternalEvent
{
    public static AttributeRegistered Create(
        Guid id,
        string name,
        AttributeType type,
        string unit,
        LookupStatus status
    ) => new(id, name, type, unit, status);
}