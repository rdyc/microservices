namespace Shipment.Carts;

public record CartCurrency(
    Guid Id,
    string Name,
    string Code,
    string Symbol
);