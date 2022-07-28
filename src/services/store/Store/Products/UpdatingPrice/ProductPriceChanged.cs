using FW.Core.Events;

namespace Store.Products.UpdatingPrice;

public record ProductPriceChanged(
    Guid ProductId,
    ProductCurrency Currency,
    decimal Price
) : IExternalEvent
{
    public static ProductPriceChanged Create(
        Guid productId,
        ProductCurrency currency,
        decimal price
    ) => new(productId, currency, price);
}