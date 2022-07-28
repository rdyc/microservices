using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Driver;
using Payment.Payments.DiscardingPayment;
using Payment.Payments.GettingPaymentById;
using Payment.Payments.TimingOutPayment;

namespace Payment.Payments.FailingPayment;

public record PaymentFailed(
    Guid OrderId,
    Guid? PaymentId,
    DateTime FailedAt,
    PaymentFailReason FailReason
) : IExternalEvent
{
    public static PaymentFailed Create(
        Guid orderId,
        Guid? paymentId,
        DateTime failedAt,
        PaymentFailReason failReason
    ) => new(orderId, paymentId, failedAt, failReason);
}


internal class TransformIntoPaymentFailed :
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
                payment!.OrderId,
                @event.Data.PaymentId,
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
                @event.Data.OrderId,
                null,
                @event.Data.TimedOutAt,
                PaymentFailReason.TimedOut
            ),
            @event.Metadata
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }
}