namespace Lookup.Currencies.RegisteringCurrency;

public record CurrencyRegistered(Guid Id, string Name, string Code, string Symbol, LookupStatus Status)
{
    public static CurrencyRegistered Create(Guid id, string name, string code, string symbol, LookupStatus status)
    {
        return new CurrencyRegistered(id, name, code, symbol, status);
    }
}