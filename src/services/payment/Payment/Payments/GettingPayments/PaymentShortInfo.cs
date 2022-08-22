using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using Payment.Payments.CompletingPayment;
using Payment.Payments.DiscardingPayment;
using Payment.Payments.RequestingPayment;
using Payment.Payments.TimingOutPayment;

namespace Payment.Payments.GettingPayments;

[BsonCollection("payment_shortinfo")]
public record PaymentShortInfo : Document
{
    [BsonElement("order_id")]
    public Guid OrderId { get; set; } = default!;

    [BsonElement("amount")]
    public decimal Amount { get; set; } = default!;

    [BsonElement("requested_at")]
    public DateTime RequestedAt { get; set; }

    [BsonElement("expired_at")]
    public DateTime ExpiredAt { get; set; }
    
    [BsonElement("status")]
    public PaymentStatus Status { get; set; }

    [BsonElement("version")]
    public ulong Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
}

internal class PaymentShortInfoProjection
{
    public static PaymentShortInfo Handle(EventEnvelope<PaymentRequested> eventEnvelope)
    {
        var (paymentId, orderId, amount, requestedAt) = eventEnvelope.Data;

        return new PaymentShortInfo
        {
            Id = paymentId,
            OrderId = orderId,
            Amount = amount,
            RequestedAt = requestedAt,
            ExpiredAt = requestedAt.AddHours(1),
            Status = PaymentStatus.Pending,
            Version = eventEnvelope.Metadata.StreamPosition,
            Position = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<PaymentCompleted> eventEnvelope, PaymentShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = PaymentStatus.Completed;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<PaymentDiscarded> eventEnvelope, PaymentShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = PaymentStatus.Discarded;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<PaymentTimedOut> eventEnvelope, PaymentShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = PaymentStatus.Failed;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }
}