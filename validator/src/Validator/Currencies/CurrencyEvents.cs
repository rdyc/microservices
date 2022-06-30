using Validator.Currencies;

namespace Lookup.Currencies.Registering
{
    public record CurrencyRegistered(Guid Id, string Name, string Code, string Symbol, CurrencyStatus Status);
}

namespace Lookup.Currencies.Modifying
{
    public record CurrencyModified(Guid Id, string Name, string Code, string Symbol);
}

namespace Lookup.Currencies.Removing
{
    public record CurrencyRemoved(Guid Id);
}