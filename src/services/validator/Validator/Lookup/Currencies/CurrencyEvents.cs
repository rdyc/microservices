namespace Validator.Lookup.Currencies;

public record CurrencyRegistered(Guid Id, string Name, string Code, string Symbol, LookupStatus Status);
public record CurrencyModified(Guid Id, string Name, string Code, string Symbol);
public record CurrencyRemoved(Guid Id);