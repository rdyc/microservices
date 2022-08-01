using Cart.Carts.ConfirmingCart;
using EventStore.Client;
using FW.Core.Events;
using FW.Core.EventStoreDB.Events;
using FW.Core.Exceptions;

namespace Cart.Carts.FinalizingCart;

public record CartFinalized(
    Guid CartId,
    Guid ClientId,
    IEnumerable<CartProduct> Products,
    decimal TotalPrice,
    DateTime FinalizedAt
) : IExternalEvent
{
    public static CartFinalized Create(
        Guid cartId,
        Guid clientId,
        IEnumerable<CartProduct> products,
        decimal totalPrice,
        DateTime finalizedAt)
    {
        return new(cartId, clientId, products, totalPrice, finalizedAt);
    }
}

internal class HandleCartFinalized : IEventHandler<EventEnvelope<CartConfirmed>>
{
    private readonly EventStoreClient eventStore;
    private readonly IEventBus eventBus;

    public HandleCartFinalized(
        EventStoreClient eventStore,
        IEventBus eventBus)
    {
        this.eventStore = eventStore;
        this.eventBus = eventBus;
    }

    public async Task Handle(EventEnvelope<CartConfirmed> @event, CancellationToken cancellationToken)
    {
        var (id, confirmedAt) = @event.Data;
        var cart = await eventStore.AggregateStream<Cart>(id, cancellationToken);

        if (cart == null)
            throw AggregateNotFoundException.For<Cart>(id);

        var externalEvent = new EventEnvelope<CartFinalized>(
            CartFinalized.Create(
                id,
                cart.ClientId,
                cart.Products,
                cart.TotalPrice,
                confirmedAt
            ),
            @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }
}
