namespace Cart.Carts;

public record CartCurrency(
    Guid Id,
    string Name,
    string Code,
    string Symbol
)
{
    public static CartCurrency Create(
        Guid id,
        string name,
        string code,
        string symbol
    ) => new(id, name, code, symbol);
};