namespace Store.Carts;

public record CartProduct(
    Guid ProductId,
    string Sku,
    string Name,
    int Quantity,
    CartCurrency Currency,
    decimal Price
);