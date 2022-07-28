using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using Payment.Payments.CompletingPayment;
using Payment.Payments.DiscardingPayment;
using Payment.Payments.RequestingPayment;
using Payment.Payments.TimingOutPayment;

namespace Payment.Payments.GettingPaymentById;

[BsonCollection("payment_details")]
public record PaymentDetails : Document
{
    [BsonElement("order_id")]
    public Guid OrderId { get; set; } = default!;

    [BsonElement("amount")]
    public decimal Amount { get; set; } = default!;

    [BsonElement("requested_at")]
    public DateTime RequestedAt { get; set; }

    [BsonElement("expired_at")]
    public DateTime? ExpiredAt { get; set; }
    
    [BsonElement("status")]
    public PaymentStatus Status { get; set; }

    [BsonElement("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [BsonElement("discarded_reason")]
    public DiscardReason? DiscardedReason { get; set; }

    [BsonElement("discarded_at")]
    public DateTime? DiscardedAt { get; set; }

    [BsonElement("version")]
    public ulong Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
}

public class PaymentDetailsProjection
{
    public static PaymentDetails Handle(EventEnvelope<PaymentRequested> eventEnvelope)
    {
        var (paymentId, orderId, amount, requestedAt) = eventEnvelope.Data;

        return new PaymentDetails
        {
            Id = paymentId,
            OrderId = orderId,
            Amount = amount,
            RequestedAt = requestedAt,
            ExpiredAt = requestedAt.AddHours(6),
            Status = PaymentStatus.Pending,
            Version = eventEnvelope.Metadata.StreamPosition,
            Position = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<PaymentCompleted> eventEnvelope, PaymentDetails view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, _, completedAt) = eventEnvelope.Data;

        view.CompletedAt = completedAt;
        view.Status = PaymentStatus.Completed;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<PaymentDiscarded> eventEnvelope, PaymentDetails view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, reason, discardedAt) = eventEnvelope.Data;

        view.DiscardedReason = reason;
        view.DiscardedAt = discardedAt;
        view.Status = PaymentStatus.Failed;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<PaymentTimedOut> eventEnvelope, PaymentDetails view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = PaymentStatus.Failed;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }
}