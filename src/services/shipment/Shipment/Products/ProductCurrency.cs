namespace Shipment.Products;

public record ProductCurrency(
    Guid Id,
    string Name,
    string Code,
    string Symbol
);