using Cart.ShoppingCarts.ConfirmingCart;
using EventStore.Client;
using FW.Core.Events;
using FW.Core.EventStoreDB.Events;
using FW.Core.Exceptions;

namespace Cart.ShoppingCarts.FinalizingCart;

public record ShoppingCartFinalized(
    Guid CartId,
    Guid ClientId,
    IEnumerable<ShoppingCartProduct> Products,
    decimal TotalPrice,
    DateTime FinalizedAt
) : IExternalEvent
{
    public static ShoppingCartFinalized Create(
        Guid cartId,
        Guid clientId,
        IEnumerable<ShoppingCartProduct> products,
        decimal totalPrice,
        DateTime finalizedAt)
    {
        return new ShoppingCartFinalized(cartId, clientId, products, totalPrice, finalizedAt);
    }
}

internal class HandleCartFinalized : IEventHandler<EventEnvelope<ShoppingCartConfirmed>>
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

    public async Task Handle(EventEnvelope<ShoppingCartConfirmed> @event, CancellationToken cancellationToken)
    {
        var cart = await eventStore.AggregateStream<ShoppingCart>(@event.Data.CartId, cancellationToken);

        if (cart == null)
            throw AggregateNotFoundException.For<ShoppingCart>(@event.Data.CartId);

        var externalEvent = new EventEnvelope<ShoppingCartFinalized>(
            ShoppingCartFinalized.Create(
                @event.Data.CartId,
                cart.ClientId,
                cart.Products,
                cart.TotalPrice,
                @event.Data.ConfirmedAt
            ),
            @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }
}
