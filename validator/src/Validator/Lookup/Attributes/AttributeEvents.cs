namespace Validator.Lookup.Attributes;

public record AttributeRegistered(Guid Id, string Name, AttributeType Type, string Unit, LookupStatus Status);
public record AttributeModified(Guid Id, string Name, AttributeType Type, string Unit);
public record AttributeRemoved(Guid Id);