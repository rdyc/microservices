namespace Store.Lookup.Currencies.ModifyingCurrency;

public record CurrencyModified(
    Guid Id,
    string Name,
    string Code,
    string Symbol
);