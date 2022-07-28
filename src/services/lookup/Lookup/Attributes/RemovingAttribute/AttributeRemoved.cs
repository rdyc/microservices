using FW.Core.Events;

namespace Lookup.Attributes.RemovingAttribute;

public record AttributeRemoved(Guid AttributeId) : IExternalEvent
{
    public static AttributeRemoved Create(Guid attributeId) =>
        new(attributeId);
}