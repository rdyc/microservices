using FW.Core.Events;

namespace Lookup.Currencies.Publishing;

public record CurrencyRegistered(Guid Id, string Name, string Code, string Symbol, CurrencyStatus Status) : IExternalEvent
{
    public static CurrencyRegistered From(Registering.CurrencyRegistered registered)
    {
        var (id, code, name, symbol, status) = registered;

        return new CurrencyRegistered(id, code, name, symbol, status);
    }
}

public record CurrencyModified(Guid Id, string Name, string Code, string Symbol) : IExternalEvent
{
    public static CurrencyModified From(Modifying.CurrencyModified modified)
    {
        var (id, code, name, symbol) = modified;

        return new CurrencyModified(id, code, name, symbol);
    }
}

public record CurrencyRemoved(Guid Id) : IExternalEvent
{
    public static CurrencyRemoved From(Removing.CurrencyRemoved removed)
    {
        return new CurrencyRemoved(removed.Id);
    }
}

internal class HandleCurrencyChanges :
    IEventHandler<EventEnvelope<CurrencyRegistered>>,
    IEventHandler<EventEnvelope<CurrencyModified>>,
    IEventHandler<EventEnvelope<CurrencyRemoved>>
{
    private readonly IEventBus eventBus;

    public HandleCurrencyChanges(IEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public async Task Handle(EventEnvelope<CurrencyRegistered> @event, CancellationToken cancellationToken)
    {
        await eventBus.Publish(@event, cancellationToken);
    }

    public async Task Handle(EventEnvelope<CurrencyRemoved> @event, CancellationToken cancellationToken)
    {
        await eventBus.Publish(@event, cancellationToken);
    }

    public async Task Handle(EventEnvelope<CurrencyModified> @event, CancellationToken cancellationToken)
    {
        await eventBus.Publish(@event, cancellationToken);
    }
}