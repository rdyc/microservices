using Cart.Pricing;
using Cart.Carts.Products;

namespace Cart.Tests.Stubs.Products;

internal class FakeProductPriceCalculator: IProductPriceCalculator
{
    public const decimal FakePrice = 13;
    public IReadOnlyList<PricedProductItem> Calculate(params ProductItem[] productItems)
    {
        return productItems
            .Select(pi => PricedProductItem.Create(pi, FakePrice))
            .ToList();
    }
}