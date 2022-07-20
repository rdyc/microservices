namespace Cart.ShoppingCarts;

public record ShoppingCartCurrency(
    Guid Id,
    string Name,
    string Code,
    string Symbol
)
{
    public static ShoppingCartCurrency Create(Guid id, string name, string code, string symbol) =>
        new(id, name, code, symbol);
};