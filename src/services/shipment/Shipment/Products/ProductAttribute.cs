using Shipment.Lookup.Attributes;

namespace Shipment.Products;

public record ProductAttribute(
    Guid Id,
    string Name,
    AttributeType Type,
    string Unit,
    string Value
);