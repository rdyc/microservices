namespace Order.Carts.FinalizingCart;

public record CartFinalized(
    Guid CartId,
    Guid ClientId,
    IEnumerable<CartProduct> Products,
    decimal TotalPrice,
    DateTime FinalizedAt
);