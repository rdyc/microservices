using FW.Core.Commands;

namespace Lookup.Currencies;

public record CurrencyCommand(Guid? Id, string Name, string Code, string Symbol, CurrencyStatus Status) : ICommand;