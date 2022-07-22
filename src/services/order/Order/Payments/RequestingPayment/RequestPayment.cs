using FW.Core.Events;
using Order.Orders.InitializingOrder;

namespace Order.Payments.RequestingPayment;

public class RequestPayment : IExternalEvent
{
    public Guid OrderId { get; }
    public decimal Amount { get; }

    private RequestPayment(Guid orderId, decimal amount)
    {
        OrderId = orderId;
        Amount = amount;
    }

    public static RequestPayment Create(Guid orderId, decimal amount)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        return new(orderId, amount);
    }
}

public class HandleRequestPayment : IEventHandler<EventEnvelope<OrderInitialized>>
{
    private readonly IEventBus eventBus;

    public HandleRequestPayment(IEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public async Task Handle(EventEnvelope<OrderInitialized> @event, CancellationToken cancellationToken)
    {
        var externalEvent = new EventEnvelope<RequestPayment>(
            RequestPayment.Create(@event.Data.OrderId, @event.Data.TotalPrice),
            @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }
}