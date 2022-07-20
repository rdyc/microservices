using FW.Core.Events;

namespace Store.Products.UpdatingStock;

public record ProductStockChanged(
    Guid Id,
    int Stock
) : IExternalEvent
{
    public static ProductStockChanged Create(Guid id, int stock) =>
        new(id, stock);
}