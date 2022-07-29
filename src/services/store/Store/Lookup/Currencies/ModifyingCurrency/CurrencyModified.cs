namespace Store.Lookup.Currencies.ModifyingCurrency;

public record CurrencyModified(
    Guid CurrencyId,
    string Name,
    string Code,
    string Symbol
);