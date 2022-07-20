using FW.Core.Events;

namespace Lookup.Currencies.RegisteringCurrency;

public record CurrencyRegistered(
    Guid Id,
    string Name,
    string Code,
    string Symbol,
    LookupStatus Status
) : IExternalEvent
{
    public static CurrencyRegistered Create(
        Guid id,
        string name,
        string code,
        string symbol,
        LookupStatus status
    ) => new (id, name, code, symbol, status);
}