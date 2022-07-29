namespace Shipment.Products.SellingProduct;

public record ProductSold(
    Guid ProductId,
    int Quantity
);