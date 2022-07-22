using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Driver;
using Payment.Payments.DiscardingPayment;
using Payment.Payments.GettingPaymentById;
using Payment.Payments.TimingOutPayment;

namespace Payment.Payments.FailingPayment;

public record PaymentFailed(
    Guid OrderId,
    Guid PaymentId,
    decimal Amount,
    DateTime FailedAt,
    PaymentFailReason FailReason
): IExternalEvent
{
    public static PaymentFailed Create(
        Guid paymentId,
        Guid orderId,
        decimal amount,
        DateTime failedAt,
        PaymentFailReason failReason
    ) => new (paymentId, orderId, amount, failedAt, failReason);
}


public class TransformIntoPaymentFailed :
    IEventHandler<EventEnvelope<PaymentDiscarded>>,
    IEventHandler<EventEnvelope<PaymentTimedOut>>
{
    private readonly IMongoCollection<PaymentDetails> collection;
    private readonly IEventBus eventBus;

    public TransformIntoPaymentFailed(
        IMongoDatabase database,
        IEventBus eventBus
    )
    {
        var collectionName = MongoHelper.GetCollectionName<PaymentDetails>();
        this.collection = database.GetCollection<PaymentDetails>(collectionName);
        this.eventBus = eventBus;
    }

    public async Task Handle(EventEnvelope<PaymentDiscarded> @event, CancellationToken cancellationToken)
    {
        var payment = await collection.Find(e => e.Id.Equals(@event.Data.PaymentId))
            .SingleAsync(cancellationToken);

        var externalEvent = new EventEnvelope<PaymentFailed>(
            PaymentFailed.Create(
                @event.Data.PaymentId,
                payment!.OrderId,
                payment.Amount,
                @event.Data.DiscardedAt,
                PaymentFailReason.Discarded
            ),
            @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }

    public async Task Handle(EventEnvelope<PaymentTimedOut> @event, CancellationToken cancellationToken)
    {
        var payment = await collection.Find(e => e.Id.Equals(@event.Data.PaymentId))
            .SingleAsync(cancellationToken);

        var externalEvent = new EventEnvelope<PaymentFailed>(
            PaymentFailed.Create(
                @event.Data.PaymentId,
                payment!.OrderId,
                payment.Amount,
                @event.Data.TimedOutAt,
                PaymentFailReason.Discarded
            ),
            @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }
}