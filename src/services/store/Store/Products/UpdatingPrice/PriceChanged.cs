using Store.Currencies;

namespace Store.Products.UpdatingPrice;

public record PriceChanged(
    Guid ProductId,
    Currency Currency,
    decimal Price
)
{
    public static PriceChanged Create(Guid productId, Currency currency, decimal price) =>
        new(productId, currency, price);
}