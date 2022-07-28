using FW.Core.Events;

namespace Lookup.Currencies.RegisteringCurrency;

public record CurrencyRegistered(
    Guid CurrencyId,
    string Name,
    string Code,
    string Symbol,
    LookupStatus Status
) : IExternalEvent
{
    public static CurrencyRegistered Create(
        Guid currencyId,
        string name,
        string code,
        string symbol,
        LookupStatus status
    ) => new (currencyId, name, code, symbol, status);
}