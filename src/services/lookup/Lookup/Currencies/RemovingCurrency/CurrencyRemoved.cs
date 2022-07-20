using FW.Core.Events;

namespace Lookup.Currencies.RemovingCurrency;

public record CurrencyRemoved(
    Guid Id
) : IExternalEvent
{
    public static CurrencyRemoved Create(Guid id) =>
        new(id);
}