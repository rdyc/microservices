using FW.Core.Events;

namespace Lookup.Currencies.Registering;

public record CurrencyRegistered(Guid Id, string Name, string Code, string Symbol, CurrencyStatus Status)
{
    public static CurrencyRegistered Create(Guid id, string name, string code, string symbol, CurrencyStatus status)
    {
        return new CurrencyRegistered(id, code, name, symbol, status);
    }
}

internal class HandleCurrencyRegistered :
    IEventHandler<CurrencyRegistered>,
    IEventHandler<EventEnvelope<CurrencyRegistered>>
{
    public Task Handle(CurrencyRegistered @event, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public Task Handle(EventEnvelope<CurrencyRegistered> @event, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}