namespace Lookup.WebApi.Requests;

public record CurrencyCreateRequest(string Name, string Code, string Symbol, LookupStatus Status);
public record CurrencyModifyRequest(string Name, string Code, string Symbol);
