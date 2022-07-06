using FW.Core.Events;
using Lookup.Attributes.ModifyingAttribute;
using Lookup.Attributes.RegisteringAttribute;
using Lookup.Attributes.RemovingAttribute;

namespace Lookup.Attributes;

internal class HandleAttributeChanged :
    IEventHandler<EventEnvelope<AttributeRegistered>>,
    IEventHandler<EventEnvelope<AttributeModified>>,
    IEventHandler<EventEnvelope<AttributeRemoved>>
{
    private readonly IEventBus eventBus;

    public HandleAttributeChanged(IEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public async Task Handle(EventEnvelope<AttributeRegistered> @event, CancellationToken cancellationToken)
    {
        var externalEvent = new EventEnvelope<LookupChanged>(
            LookupChanged.Create(Context.Attribute, State.Added, @event.Data),
            @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }

    public async Task Handle(EventEnvelope<AttributeModified> @event, CancellationToken cancellationToken)
    {
        var externalEvent = new EventEnvelope<LookupChanged>(
           LookupChanged.Create(Context.Attribute, State.Updated, @event.Data),
           @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }

    public async Task Handle(EventEnvelope<AttributeRemoved> @event, CancellationToken cancellationToken)
    {
        var externalEvent = new EventEnvelope<LookupChanged>(
           LookupChanged.Create(Context.Attribute, State.Deleted, @event.Data),
           @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }
}