using Store.Lookup;

namespace Store.Products.UpdatingPrice;

public record ProductPriceChanged(
    Guid Id,
    CurrencyPrice Currency,
    decimal Price
)
{
    public static ProductPriceChanged Create(Guid id, CurrencyPrice currency, decimal price) =>
        new(id, currency, price);
}


public record CurrencyPrice(Guid Id, string Name, string Code, string Symbol, LookupStatus Status);