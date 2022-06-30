namespace Lookup.Currencies.Modifying;

public record CurrencyModified(Guid Id, string Name, string Code, string Symbol)
{
    public static CurrencyModified Create(Guid id, string name, string code, string symbol)
    {
        return new CurrencyModified(id, name, code, symbol);
    }
}