using FW.Core.Events;

namespace Lookup.Currencies.ModifyingCurrency;

public record CurrencyModified(
    Guid Id,
    string Name,
    string Code,
    string Symbol
) : IExternalEvent
{
    public static CurrencyModified Create(
        Guid id,
        string name,
        string code,
        string symbol
    ) => new(id, name, code, symbol);
}