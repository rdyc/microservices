using Shipment.Lookup.Attributes;

namespace Shipment.Products.RemovingAttribute;

public record ProductAttributeRemoved(
    Guid ProductId,
    Guid AttributeId,
    string Name,
    AttributeType Type,
    string Unit,
    string Value
);