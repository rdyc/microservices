using Cart.Products;

namespace Cart.Carts;

public record CartProduct(
    Guid ProductId,
    string Sku,
    string Name,
    int Quantity,
    CartCurrency Currency,
    decimal Price
) : IProduct
{
    public decimal TotalPrice => Quantity * Price;

    public static CartProduct From(
        Guid? productId,
        string sku,
        string name,
        int? quantity,
        CartCurrency? currency,
        decimal? price)
    {
        if (!productId.HasValue)
            throw new ArgumentNullException(nameof(productId));

        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        if (currency is null)
            throw new ArgumentNullException(nameof(currency));

        var _price = price switch
        {
            null => throw new ArgumentNullException(nameof(price)),
            <= 0 => throw new ArgumentOutOfRangeException(nameof(quantity), "Price has to be a positive number"),
            _ => price.Value
        };

        return quantity switch
        {
            null => throw new ArgumentNullException(nameof(quantity)),
            <= 0 => throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity has to be a positive number"),
            _ => new CartProduct(productId.Value, sku, name, quantity.Value, currency, _price)
        };
    }

    public CartProduct MergeWith(CartProduct product)
    {
        if (!MatchesProduct(product))
            throw new ArgumentException("Product does not match.");

        return From(ProductId, product.Sku, product.Name, Quantity + product.Quantity, product.Currency, product.Price);
    }

    public CartProduct Substract(CartProduct product)
    {
        if (!MatchesProduct(product))
            throw new ArgumentException("Product does not match.");

        return From(ProductId, product.Sku, product.Name, Quantity - product.Quantity, product.Currency, product.Price);
    }

    public bool MatchesProduct(CartProduct product)
    {
        return ProductId == product.ProductId;
    }

    public bool MatchesProductAndPrice(CartProduct product)
    {
        return ProductId == product.ProductId && Price == product.Price;
    }

    public bool HasEnough(int quantity)
    {
        return Quantity >= quantity;
    }

    public bool HasTheSameQuantity(CartProduct product)
    {
        return Quantity == product.Quantity;
    }
}