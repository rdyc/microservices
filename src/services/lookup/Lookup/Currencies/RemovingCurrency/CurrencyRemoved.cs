using FW.Core.Events;

namespace Lookup.Currencies.RemovingCurrency;

public record CurrencyRemoved(
    Guid CurrencyId
) : IExternalEvent
{
    public static CurrencyRemoved Create(Guid currencyid) =>
        new(currencyid);
}