namespace Order.Carts;

public record CartCurrency(
    Guid Id,
    string Name,
    string Code,
    string Symbol
);