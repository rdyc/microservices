namespace Store.Lookup.Currencies.RegisteringCurrency;

public record CurrencyRegistered(
    Guid CurrencyId,
    string Name,
    string Code,
    string Symbol,
    LookupStatus Status
);