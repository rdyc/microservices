namespace Shipment.Orders;

public record OrderPaymentRecorded(
    Guid ClientId,
    Guid OrderId,
    Guid PaymentId,
    IEnumerable<ShoppingCartProduct> Products,
    decimal TotalPrice,
    DateTime RecordedAt
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