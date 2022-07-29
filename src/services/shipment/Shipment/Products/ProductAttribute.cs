using Shipment.Lookup.Attributes;

namespace Cart.Products;

public record ProductAttribute(
    Guid Id,
    string Name,
    AttributeType Type,
    string Unit,
    string Value
);