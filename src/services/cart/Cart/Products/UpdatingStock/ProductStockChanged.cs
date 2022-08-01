namespace Cart.Products.UpdatingStock;

public record ProductStockChanged(
    Guid Id,
    int Stock
);