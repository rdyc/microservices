namespace Store.Lookup.Attributes.RegisteringAttribute;

public record AttributeRegistered(
    Guid Id,
    string Name,
    AttributeType Type,
    string Unit,
    LookupStatus Status
);