using FW.Core.Events;

namespace Lookup.Attributes.RemovingAttribute;

public record AttributeRemoved(Guid Id) : IExternalEvent
{
    public static AttributeRemoved Create(Guid id) =>
        new(id);
}