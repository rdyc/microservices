namespace Store.Lookup.Attributes;

public record AttributeRegistered(Guid AttributeId, string Name, AttributeType Type, string Unit, LookupStatus Status);
public record AttributeModified(Guid AttributeId, string Name, AttributeType Type, string Unit);
public record AttributeRemoved(Guid AttributeId);