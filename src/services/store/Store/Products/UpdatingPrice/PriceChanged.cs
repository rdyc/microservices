using Store.Currencies;

namespace Store.Products.UpdatingPrice;

public record PriceChanged(
    Guid Id,
    Currency Currency,
    decimal Price
)
{
    public static PriceChanged Create(Guid id, Currency currency, decimal price) =>
        new(id, currency, price);
}