namespace Lookup.Currencies.Registering;

public record CurrencyRegistered(Guid Id, string Name, string Code, string Symbol, CurrencyStatus Status)
{
    public static CurrencyRegistered Create(Guid id, string name, string code, string symbol, CurrencyStatus status)
    {
        return new CurrencyRegistered(id, name, code, symbol, status);
    }
}