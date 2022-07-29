namespace Shipment.Products.ShippingProduct;

public record ProductShipped(
    Guid ProductId,
    int Quantity
);