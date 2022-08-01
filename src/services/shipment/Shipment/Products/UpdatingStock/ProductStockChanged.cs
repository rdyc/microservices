namespace Shipment.Products.UpdatingStock;

public record ProductStockChanged(
    Guid Id,
    int Stock
);