namespace Cart.Products.UpdatingPrice;

public record ProductPriceChanged(
    Guid ProductId,
    ProductCurrency Currency,
    decimal Price
);