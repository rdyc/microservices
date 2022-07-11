using FW.Core.Events;
using Lookup.Currencies.ModifyingCurrency;
using Lookup.Currencies.RegisteringCurrency;
using Lookup.Currencies.RemovingCurrency;

namespace Lookup.Currencies;

internal class HandleCurrencyChanged :
    IEventHandler<EventEnvelope<CurrencyRegistered>>,
    IEventHandler<EventEnvelope<CurrencyModified>>,
    IEventHandler<EventEnvelope<CurrencyRemoved>>
{
    private readonly IEventBus eventBus;

    public HandleCurrencyChanged(IEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public async Task Handle(EventEnvelope<CurrencyRegistered> @event, CancellationToken cancellationToken)
    {
        var externalEvent = new EventEnvelope<LookupChanged>(
            LookupChanged.Create(Context.Currency, State.Added, @event.Data),
            @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }

    public async Task Handle(EventEnvelope<CurrencyModified> @event, CancellationToken cancellationToken)
    {
        var externalEvent = new EventEnvelope<LookupChanged>(
           LookupChanged.Create(Context.Currency, State.Updated, @event.Data),
           @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }

    public async Task Handle(EventEnvelope<CurrencyRemoved> @event, CancellationToken cancellationToken)
    {
        var externalEvent = new EventEnvelope<LookupChanged>(
           LookupChanged.Create(Context.Currency, State.Deleted, @event.Data),
           @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }
}