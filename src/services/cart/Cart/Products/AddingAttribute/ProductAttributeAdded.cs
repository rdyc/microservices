using Cart.Lookup.Attributes;

namespace Cart.Products.AddingAttribute;

public record ProductAttributeAdded(
    Guid ProductId,
    Guid AttributeId,
    string Name,
    AttributeType Type,
    string Unit,
    string Value
);