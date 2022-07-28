namespace Store.Lookup.Currencies;

public record CurrencyRegistered(Guid CurrencyId, string Name, string Code, string Symbol, LookupStatus Status);
public record CurrencyModified(Guid CurrencyId, string Name, string Code, string Symbol);
public record CurrencyRemoved(Guid CurrencyId);