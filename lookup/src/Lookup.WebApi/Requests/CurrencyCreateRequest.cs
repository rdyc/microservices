using Lookup.Currencies;

namespace Lookup.WebApi.Requests;

public record CurrencyCreateRequest(string Name, string Code, string Symbol, CurrencyStatus Status);