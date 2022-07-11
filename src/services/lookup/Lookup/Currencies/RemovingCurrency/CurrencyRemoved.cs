namespace Lookup.Currencies.RemovingCurrency;

public record CurrencyRemoved(Guid Id)
{
    public static CurrencyRemoved Create(Guid id)
    {
        return new CurrencyRemoved(id);
    }
}