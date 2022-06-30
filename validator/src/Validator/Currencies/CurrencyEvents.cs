using Validator.Currencies;

namespace Lookup.Currencies.Registering
{
    public record CurrencyRegistered(Guid Id, string Name, string Code, string Symbol, CurrencyStatus Status)
    {
        public static CurrencyRegistered Create(Guid id, string name, string code, string symbol, CurrencyStatus status)
        {
            return new CurrencyRegistered(id, code, name, symbol, status);
        }
    }
}

namespace Lookup.Currencies.Modifying
{
    public record CurrencyModified(Guid Id, string Name, string Code, string Symbol)
    {
        public static CurrencyModified Create(Guid id, string name, string code, string symbol)
        {
            return new CurrencyModified(id, name, code, symbol);
        }
    }
}

namespace Lookup.Currencies.Removing
{
    public record CurrencyRemoved(Guid Id)
    {
        public static CurrencyRemoved Create(Guid id)
        {
            return new CurrencyRemoved(id);
        }
    }
}