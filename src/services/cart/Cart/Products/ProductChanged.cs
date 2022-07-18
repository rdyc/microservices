using FW.Core.Commands;
using FW.Core.Events;
using Newtonsoft.Json.Linq;

namespace Cart.Products;

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

internal class HandleProductChanged : IEventHandler<EventEnvelope<ProductChanged>>
{
    private readonly ICommandBus commandBus;

    public HandleProductChanged(ICommandBus commandBus)
    {
        this.commandBus = commandBus;
    }

    public Task Handle(EventEnvelope<ProductChanged> @event, CancellationToken cancellationToken)
    {
        var (data, _) = @event;
        var jObject = (JObject)data.Data;

        if (jObject == null)
            throw new InvalidOperationException("An event object should not null");

        return data.State switch
        {
            State.Added => Dispatch(ToEvent<ProductRegistered>(jObject)),
            State.Updated => Dispatch(ToEvent<ProductModified>(jObject)),
            State.Deleted => Dispatch(ToEvent<ProductRemoved>(jObject)),
            _ => throw new InvalidOperationException($"Unmathced event handler for state {data.State}")
        };
    }

    private static T ToEvent<T>(JObject jObject)
    {
        return jObject.ToObject<T>() ?? throw new InvalidCastException($"Nullable object result for {typeof(T)}");
    }

    private async Task Dispatch(ProductRegistered @event)
    {
        var (id, sku, name, status) = @event;
        await commandBus.Send(CreateProduct.Create(id, sku, name, status));
    }

    private async Task Dispatch(ProductModified @event)
    {
        var (id, sku, name) = @event;
        await commandBus.Send(UpdateProduct.Create(id, sku, name));
    }

    private async Task Dispatch(ProductRemoved @event)
    {
        await commandBus.Send(DeleteProduct.Create(@event.Id));
    }
}