namespace Store.Lookup.Attributes.ModifyingAttribute;

public record AttributeModified(
    Guid AttributeId,
    string Name,
    AttributeType Type,
    string Unit
);