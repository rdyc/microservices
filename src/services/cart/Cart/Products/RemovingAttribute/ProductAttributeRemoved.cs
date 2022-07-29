using Cart.Lookup.Attributes;

namespace Cart.Products.RemovingAttribute;

public record ProductAttributeRemoved(
    Guid ProductId,
    Guid AttributeId,
    string Name,
    AttributeType Type,
    string Unit,
    string Value
);