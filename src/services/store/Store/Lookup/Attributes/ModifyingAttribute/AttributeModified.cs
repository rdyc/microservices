namespace Store.Lookup.Attributes.ModifyingAttribute;

public record AttributeModified(
    Guid Id,
    string Name,
    AttributeType Type,
    string Unit
);