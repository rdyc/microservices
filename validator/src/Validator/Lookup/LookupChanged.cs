using FW.Core.Commands;
using FW.Core.Events;
using Validator.Lookup.Attributes;
using Validator.Lookup.Currencies;

namespace Validator.Lookup;

public record LookupChanged : IExternalEvent
{
    private LookupChanged(Context context, State state, object data)
    {
        Context = context;
        State = state;
        Data = data;
    }

    public static LookupChanged Create(Context context, State state, object data) =>
        new(context, state, data);

    public Context Context { get; }
    public State State { get; }
    public object Data { get; }
}

public enum Context
{
    Attribute, Currency
}

public enum State
{
    Added, Updated, Deleted
}

internal class HandleLookupChanged : IEventHandler<EventEnvelope<LookupChanged>>
{
    private readonly ICommandBus commandBus;

    public HandleLookupChanged(ICommandBus commandBus)
    {
        this.commandBus = commandBus;
    }

    public Task Handle(EventEnvelope<LookupChanged> @event, CancellationToken cancellationToken)
    {
        var (data, _) = @event;

        return (data.Context, data.State) switch
        {
            // Todo object data cast did not work!
            (Context.Attribute, State.Added) => Dispatch((AttributeRegistered)data.Data),
            (Context.Attribute, State.Updated) => Dispatch((AttributeModified)data.Data),
            (Context.Attribute, State.Deleted) => Dispatch((AttributeRemoved)data.Data),
            (Context.Currency, State.Added) => Dispatch((CurrencyRegistered)data.Data),
            (Context.Currency, State.Updated) => Dispatch((CurrencyModified)data.Data),
            (Context.Currency, State.Deleted) => Dispatch((CurrencyRemoved)data.Data),
            (_, _) => throw new InvalidOperationException($"Unmathced event handler conditions for context {data.Context} and state {data.State}.")
        };
    }

    private async Task Dispatch(AttributeRegistered @event)
    {
        var (id, name, type, unit, status) = @event;
        await commandBus.Send(CreateAttribute.Create(id, name, type, unit, status));
    }

    private async Task Dispatch(AttributeModified @event)
    {
        var (id, name, type, unit) = @event;
        await commandBus.Send(UpdateAttribute.Create(id, name, type, unit));
    }

    private async Task Dispatch(AttributeRemoved @event) =>
        await commandBus.Send(DeleteAttribute.Create(@event.Id));

    private async Task Dispatch(CurrencyRegistered @event)
    {
        var (id, name, code, symbol, status) = @event;
        await commandBus.Send(CreateCurrency.Create(id, name, code, symbol, status));
    }

    private async Task Dispatch(CurrencyModified @event)
    {
        var (id, name, code, symbol) = @event;
        await commandBus.Send(UpdateCurrency.Create(id, name, code, symbol));
    }

    private async Task Dispatch(CurrencyRemoved @event) =>
        await commandBus.Send(DeleteCurrency.Create(@event.Id));
}