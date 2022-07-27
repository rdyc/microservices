namespace Order.ShoppingCarts.FinalizingCart;

public record ShoppingCartFinalized(
    Guid CartId,
    Guid ClientId,
    IEnumerable<ShoppingCartProduct> Products,
    decimal TotalPrice,
    DateTime FinalizedAt
);

public record ShoppingCartProduct(
    Guid ProductId,
    string Sku,
    string Name,
    int Quantity,
    ShoppingCartCurrency Currency,
    decimal Price
);

public record ShoppingCartCurrency(
    Guid Id,
    string Name,
    string Code,
    string Symbol
);