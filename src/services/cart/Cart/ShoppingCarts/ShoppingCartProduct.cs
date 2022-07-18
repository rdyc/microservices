using Cart.Products;

namespace Cart.ShoppingCarts;

public record ShoppingCartProduct(
    Guid ProductId,
    string Sku,
    string Name,
    int Quantity,
    decimal Price
) : IProduct
{
    public decimal TotalPrice => Quantity * TotalPrice;

    public static ShoppingCartProduct From(Guid? productId, string sku, string name, decimal? price, int? quantity)
    {
        if (!productId.HasValue)
            throw new ArgumentNullException(nameof(productId));

        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

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
            _ => new ShoppingCartProduct(productId.Value, sku, name, quantity.Value, _price)
        };
    }

    public ShoppingCartProduct MergeWith(ShoppingCartProduct product)
    {
        if (!MatchesProduct(product))
            throw new ArgumentException("Product does not match.");

        return From(ProductId, product.Sku, product.Name, product.Price, Quantity + product.Quantity);
    }

    public ShoppingCartProduct Substract(ShoppingCartProduct product)
    {
        if (!MatchesProduct(product))
            throw new ArgumentException("Product does not match.");

        return From(ProductId, product.Sku, product.Name, product.Price, Quantity - product.Quantity);
    }

    public bool MatchesProduct(ShoppingCartProduct product)
    {
        return ProductId == product.ProductId;
    }

    public bool MatchesProductAndPrice(ShoppingCartProduct product)
    {
        return ProductId == product.ProductId && Price == product.Price;
    }

    public bool HasEnough(int quantity)
    {
        return Quantity >= quantity;
    }

    public bool HasTheSameQuantity(ShoppingCartProduct product)
    {
        return Quantity == product.Quantity;
    }
}