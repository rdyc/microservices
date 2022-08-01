namespace Store.Lookup.Currencies.RegisteringCurrency;

public record CurrencyRegistered(
    Guid Id,
    string Name,
    string Code,
    string Symbol,
    LookupStatus Status
);