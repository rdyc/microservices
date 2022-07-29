using Shipment.Lookup.Attributes;

namespace Shipment.Products.AddingAttribute;

public record ProductAttributeAdded(
    Guid ProductId,
    Guid AttributeId,
    string Name,
    AttributeType Type,
    string Unit,
    string Value
);