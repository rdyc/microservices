namespace Store.Lookup.Attributes.RegisteringAttribute;

public record AttributeRegistered(
    Guid AttributeId,
    string Name,
    AttributeType Type,
    string Unit,
    LookupStatus Status
);