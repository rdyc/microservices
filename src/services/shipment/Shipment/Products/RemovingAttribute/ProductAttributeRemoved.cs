namespace Shipment.Products.RemovingAttribute;

public record ProductAttributeRemoved(
    Guid Id,
    ProductAttribute Attribute
);