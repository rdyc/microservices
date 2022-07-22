using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using Order.Orders.CancellingOrder;
using Order.Orders.CompletingOrder;
using Order.Orders.InitializingOrder;
using Order.Orders.RecordingOrderPayment;

namespace Order.Orders.GettingOrders;

[BsonCollection("order_shortinfo")]
public record OrderShortInfo : Document
{
    [BsonElement("client_id")]
    public Guid ClientId { get; set; }

    [BsonElement("total_products")]
    public int TotalProducts { get; set; }

    [BsonElement("total_price")]
    public decimal TotalPrice { get; set; }

    [BsonElement("status")]
    public OrderStatus Status { get; set; }

    [BsonElement("initialized_at")]
    public DateTime InitializedAt { get; set; }

    [BsonElement("payment_id")]
    public Guid? PaymentId { get; set; }

    [BsonElement("paid_at")]
    public DateTime? PaidAt { get; set; }

    [BsonElement("cancellation_reason")]
    public OrderCancellationReason? CancellationReason { get; set; }

    [BsonElement("cancelled_at")]
    public DateTime? CancelledAt { get; set; }

    [BsonElement("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [BsonElement("version")]
    public ulong Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
}

internal static class OrderShortInfoProjection
{
    public static OrderShortInfo Handle(EventEnvelope<OrderInitialized> eventEnvelope)
    {
        var (orderId, clientId, products, totalPrice, initAt) = eventEnvelope.Data;

        return new OrderShortInfo
        {
            Id = orderId,
            ClientId = clientId,
            TotalProducts = products.Count(),
            TotalPrice = totalPrice,
            InitializedAt = initAt,
            Status = OrderStatus.Opened,
            Version = eventEnvelope.Metadata.StreamPosition,
            Position = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<OrderPaymentRecorded> eventEnvelope, OrderShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, paymentId, _, _, recordedAt) = eventEnvelope.Data;

        view.PaymentId = paymentId;
        view.PaidAt = recordedAt;
        view.Status = OrderStatus.Paid;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<OrderCancelled> eventEnvelope, OrderShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, _, cancellationReason, cancelledAt) = eventEnvelope.Data;

        view.CancellationReason = cancellationReason;
        view.CancelledAt = cancelledAt;
        view.Status = OrderStatus.Cancelled;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<OrderCompleted> eventEnvelope, OrderShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, completedAt) = eventEnvelope.Data;

        view.CompletedAt = completedAt;
        view.Status = OrderStatus.Completed;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }
}