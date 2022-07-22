using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Driver;
using Payment.Payments.CompletingPayment;
using Payment.Payments.GettingPaymentById;

namespace Payment.Payments.FinalizingPayment;

public record PaymentFinalized(
    Guid OrderId,
    Guid PaymentId,
    decimal Amount,
    DateTime FinalizedAt
) : IExternalEvent
{
    public static PaymentFinalized Create(Guid paymentId, Guid orderId, decimal amount, DateTime finalizedAt)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));
        if (finalizedAt == default)
            throw new ArgumentOutOfRangeException(nameof(finalizedAt));

        return new(paymentId, orderId, amount, finalizedAt);
    }
}

public class TransformIntoPaymentFinalized : IEventHandler<EventEnvelope<PaymentCompleted>>
{
    private readonly IMongoCollection<PaymentDetails> collection;
    private readonly IEventBus eventBus;

    public TransformIntoPaymentFinalized(
        IMongoDatabase database,
        IEventBus eventBus
    )
    {
        var collectionName = MongoHelper.GetCollectionName<PaymentDetails>();
        this.collection = database.GetCollection<PaymentDetails>(collectionName);
        this.eventBus = eventBus;
    }

    public async Task Handle(EventEnvelope<PaymentCompleted> @event, CancellationToken cancellationToken)
    {
        var (paymentId, completedAt) = @event.Data;

        var payment = await collection.Find(e => e.Id.Equals(paymentId))
            .SingleAsync(cancellationToken);

        var externalEvent = new EventEnvelope<PaymentFinalized>(
            PaymentFinalized.Create(
                paymentId,
                payment!.OrderId,
                payment.Amount,
                completedAt
            ),
            @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }
}