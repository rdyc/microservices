namespace Store.Products;

public record ProductCurrency(
    Guid Id,
    string Name,
    string Code,
    string Symbol
)
{
    public static ProductCurrency Create(Guid id, string name, string code, string symbol) =>
        new(id, name, code, symbol);
}