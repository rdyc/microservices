using FW.Core.Events;
using Payment.Payments.CompletingPayment;

namespace Payment.Payments.FinalizingPayment;

public record PaymentFinalized(
    Guid PaymentId,
    Guid OrderId,
    DateTime FinalizedAt
) : IExternalEvent
{
    public static PaymentFinalized Create(Guid paymentId, Guid orderId, DateTime finalizedAt)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentNullException(nameof(paymentId));
        if (orderId == Guid.Empty)
            throw new ArgumentNullException(nameof(orderId));
        if (finalizedAt == default)
            throw new InvalidOperationException(nameof(finalizedAt));

        return new(paymentId, orderId, finalizedAt);
    }
}

internal class TransformIntoPaymentFinalized : IEventHandler<EventEnvelope<PaymentCompleted>>
{
    private readonly IEventBus eventBus;

    public TransformIntoPaymentFinalized(IEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public async Task Handle(EventEnvelope<PaymentCompleted> @event, CancellationToken cancellationToken)
    {
        var (paymentId, orderId, completedAt) = @event.Data;

        var externalEvent = new EventEnvelope<PaymentFinalized>(
            PaymentFinalized.Create(
                paymentId,
                orderId,
                completedAt
            ),
            @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }
}