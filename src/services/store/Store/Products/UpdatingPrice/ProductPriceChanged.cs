using FW.Core.Events;

namespace Store.Products.UpdatingPrice;

public record ProductPriceChanged(
    Guid Id,
    ProductCurrency Currency,
    decimal Price
) : IExternalEvent
{
    public static ProductPriceChanged Create(
        Guid id,
        ProductCurrency currency,
        decimal price
    ) => new(id, currency, price);
}