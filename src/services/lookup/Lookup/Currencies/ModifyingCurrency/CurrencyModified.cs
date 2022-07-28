using FW.Core.Events;

namespace Lookup.Currencies.ModifyingCurrency;

public record CurrencyModified(
    Guid CurrencyId,
    string Name,
    string Code,
    string Symbol
) : IExternalEvent
{
    public static CurrencyModified Create(
        Guid currencyId,
        string name,
        string code,
        string symbol
    ) => new(currencyId, name, code, symbol);
}