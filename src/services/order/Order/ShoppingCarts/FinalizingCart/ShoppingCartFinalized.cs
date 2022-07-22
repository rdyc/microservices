using FW.Core.Events;

namespace Order.ShoppingCarts.FinalizingCart;

public record ShoppingCartFinalized(
    Guid CartId,
    Guid ClientId,
    IEnumerable<ShoppingCartProduct> Products,
    decimal TotalPrice,
    DateTime FinalizedAt
) : IExternalEvent
{
    public static ShoppingCartFinalized Create(
        Guid cartId,
        Guid clientId,
        IEnumerable<ShoppingCartProduct> products,
        decimal totalPrice,
        DateTime finalizedAt
    ) => new(cartId, clientId, products, totalPrice, finalizedAt);
}

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