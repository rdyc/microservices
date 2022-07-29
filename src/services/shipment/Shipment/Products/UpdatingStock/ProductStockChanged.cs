namespace Cart.Products.UpdatingStock;

public record ProductStockChanged(
    Guid ProductId,
    int Stock
);