using MediatR;

namespace Lookup.Currencies;

public record CurrencyCommand(Guid? Id, string Name, string Code, string Symbol) : IRequest<Guid>;