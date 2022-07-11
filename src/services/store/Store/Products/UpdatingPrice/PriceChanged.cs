using Store.Currencies;

namespace Store.Products.UpdatingPrice;

public record PriceChanged(
    Currency Currency,
    decimal Price
)
{
    public static PriceChanged Create(Currency currency, decimal price) =>
        new(currency, price);
}