namespace Lookup.Currencies.RegisterCurrency;

public record CurrencyRegistered(Guid Id, string Name, string Code, string Symbol)
{
    public static CurrencyRegistered Create(Guid id, string name, string code, string symbol)
    {
        return new CurrencyRegistered(id, code, name, symbol);
    }
}