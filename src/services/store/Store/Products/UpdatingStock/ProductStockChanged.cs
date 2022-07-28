using FW.Core.Events;

namespace Store.Products.UpdatingStock;

public record ProductStockChanged(
    Guid ProductId,
    int Stock
) : IExternalEvent
{
    public static ProductStockChanged Create(
        Guid productId,
        int stock
    ) => new(productId, stock);
}