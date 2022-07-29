using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using Order.Orders.CancellingOrder;
using Order.Orders.CompletingOrder;
using Order.Orders.InitializingOrder;
using Order.Orders.RecordingOrderPayment;

namespace Order.Orders.GettingOrderHistory;

[BsonCollection("order_history")]
public record OrderHistory : Document
{
    [BsonElement("aggregate_id")]
    public Guid AggregateId { get; set; } = default!;

    [BsonElement("description")]
    public string Description { get; set; } = default!;

    [BsonElement("version")]
    public ulong Version { get; set; } = default!;

    [BsonElement("position")]
    public ulong Position { get; set; } = default!;

    public static OrderHistory Create(Guid aggregateId, string description, EventMetadata metadata)
    {
        var (eventId, streamPosition, logPosition, _) = metadata;

        return new OrderHistory
        {
            Id = Guid.Parse(eventId),
            AggregateId = aggregateId,
            Description = description,
            Version = streamPosition,
            Position = logPosition
        };
    }
}

public class OrderHistoryProjection
{
    public static OrderHistory Handle(EventEnvelope<OrderInitialized> eventEnvelope)
    {
        var (orderId, clientId, products, price, InitializedAt) = eventEnvelope.Data;

        return OrderHistory.Create(
            orderId,
            $"Initialized at {InitializedAt}, clientId: {clientId}, product: {products.Count()} and price: {price}",
            eventEnvelope.Metadata
        );
    }

    public static OrderHistory Handle(EventEnvelope<OrderPaymentRecorded> eventEnvelope)
    {
        var (_, _, paymentId, _, _, recordedAt) = eventEnvelope.Data;

        return OrderHistory.Create(
            eventEnvelope.Data.OrderId,
            $"Paid at: {recordedAt} with paymentId: {paymentId}",
            eventEnvelope.Metadata
        );
    }

    public static OrderHistory Handle(EventEnvelope<OrderCancelled> eventEnvelope)
    {
        var (_, _, reason, cancelledAt) = eventEnvelope.Data;

        return OrderHistory.Create(
            eventEnvelope.Data.OrderId,
            $"Canceled at: {cancelledAt} with reason: {reason}",
            eventEnvelope.Metadata
        );
    }

    public static OrderHistory Handle(EventEnvelope<OrderCompleted> eventEnvelope) =>
        OrderHistory.Create(
            eventEnvelope.Data.OrderId,
            $"Completed at {eventEnvelope.Data.CompletedAt}",
            eventEnvelope.Metadata
        );
}