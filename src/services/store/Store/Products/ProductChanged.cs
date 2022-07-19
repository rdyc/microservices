using FW.Core.Events;
using Store.Products.AddingAttribute;
using Store.Products.ModifyingProduct;
using Store.Products.RegisteringProduct;
using Store.Products.RemovingAttribute;
using Store.Products.RemovingProduct;
using Store.Products.UpdatingPrice;
using Store.Products.UpdatingStock;

namespace Store.Products;

public record ProductChanged : IExternalEvent
{
    private ProductChanged(State state, object data)
    {
        State = state;
        Data = data;
    }

    public static ProductChanged Create(State state, object data)
        => new(state, data);

    public State State { get; }
    public object Data { get; }
}

public enum State
{
    Added, Updated, Deleted
}

internal class HandleProductChanged :
    IEventHandler<EventEnvelope<ProductRegistered>>,
    IEventHandler<EventEnvelope<ProductModified>>,
    IEventHandler<EventEnvelope<ProductAttributeAdded>>,
    IEventHandler<EventEnvelope<ProductAttributeRemoved>>,
    IEventHandler<EventEnvelope<ProductPriceChanged>>,
    IEventHandler<EventEnvelope<ProductStockChanged>>,
    IEventHandler<EventEnvelope<ProductRemoved>>
{
    private readonly IEventBus eventBus;

    public HandleProductChanged(IEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public async Task Handle(EventEnvelope<ProductRegistered> @event, CancellationToken cancellationToken)
    {
        var externalEvent = new EventEnvelope<ProductChanged>(
            ProductChanged.Create(State.Added, @event.Data),
            @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }

    public async Task Handle(EventEnvelope<ProductModified> @event, CancellationToken cancellationToken)
    {
        var externalEvent = new EventEnvelope<ProductChanged>(
           ProductChanged.Create(State.Updated, @event.Data),
           @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }

    public async Task Handle(EventEnvelope<ProductAttributeAdded> @event, CancellationToken cancellationToken)
    {
        var externalEvent = new EventEnvelope<ProductChanged>(
           ProductChanged.Create(State.Updated, @event.Data),
           @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }

    public async Task Handle(EventEnvelope<ProductAttributeRemoved> @event, CancellationToken cancellationToken)
    {
        var externalEvent = new EventEnvelope<ProductChanged>(
           ProductChanged.Create(State.Updated, @event.Data),
           @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }

    public async Task Handle(EventEnvelope<ProductPriceChanged> @event, CancellationToken cancellationToken)
    {
        var externalEvent = new EventEnvelope<ProductChanged>(
           ProductChanged.Create(State.Updated, @event.Data),
           @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }

    public async Task Handle(EventEnvelope<ProductStockChanged> @event, CancellationToken cancellationToken)
    {
        var externalEvent = new EventEnvelope<ProductChanged>(
           ProductChanged.Create(State.Updated, @event.Data),
           @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }

    public async Task Handle(EventEnvelope<ProductRemoved> @event, CancellationToken cancellationToken)
    {
        var externalEvent = new EventEnvelope<ProductChanged>(
           ProductChanged.Create(State.Deleted, @event.Data),
           @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }
}