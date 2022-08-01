namespace Shipment.Products.UpdatingPrice;

public record ProductPriceChanged(
    Guid Id,
    ProductCurrency Currency,
    decimal Price
);