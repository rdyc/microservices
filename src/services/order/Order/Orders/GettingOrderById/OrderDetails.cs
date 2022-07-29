using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using Order.Orders.CancellingOrder;
using Order.Orders.CompletingOrder;
using Order.Orders.InitializingOrder;
using Order.Orders.ProcessingOrder;
using Order.Orders.RecordingOrderPayment;

namespace Order.Orders.GettingOrderById;

[BsonCollection("order_details")]
public record OrderDetails : Document
{
    [BsonElement("client_id")]
    public Guid ClientId { get; set; }

    [BsonElement("products")]
    public IList<OrderDetailProduct> Products { get; set; } = default!;

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

public record OrderDetailProduct : Document
{
    public OrderDetailProduct(
        Guid productId,
        string sku,
        string name,
        int quantity,
        OrderDetailCurrency currency,
        decimal price)
    {
        Id = productId;
        Sku = sku;
        Name = name;
        Quantity = quantity;
        Currency = currency;
        Price = price;
    }

    [BsonElement("sku")]
    public string Sku { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("currency")]
    public OrderDetailCurrency Currency { get; set; }

    [BsonElement("position")]
    public decimal Price { get; set; }
}

public record OrderDetailCurrency : Document
{
    public OrderDetailCurrency(
        Guid id,
        string name,
        string code,
        string symbol)
    {
        Id = id;
        Name = name;
        Code = code;
        Symbol = symbol;
    }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("code")]
    public string Code { get; set; }

    [BsonElement("symbol")]
    public string Symbol { get; set; }
}

internal static class OrderDetailsProjection
{
    public static OrderDetails Handle(EventEnvelope<OrderInitialized> eventEnvelope)
    {
        var (orderId, clientId, products, totalPrice, initAt) = eventEnvelope.Data;

        return new OrderDetails
        {
            Id = orderId,
            ClientId = clientId,
            Products = products.Select(e => 
                new OrderDetailProduct(
                    e.ProductId,
                    e.Sku,
                    e.Name,
                    e.Quantity,
                    new OrderDetailCurrency(
                        e.Currency.Id,
                        e.Currency.Name,
                        e.Currency.Code,
                        e.Currency.Symbol),
                    e.Price)
            ).ToList(),
            TotalPrice = totalPrice,
            InitializedAt = initAt,
            Status = OrderStatus.Opened,
            Version = eventEnvelope.Metadata.StreamPosition,
            Position = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<OrderPaymentRecorded> eventEnvelope, OrderDetails view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, _, paymentId, _, _, recordedAt) = eventEnvelope.Data;

        view.PaymentId = paymentId;
        view.PaidAt = recordedAt;
        view.Status = OrderStatus.Paid;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<OrderProcessed> eventEnvelope, OrderDetails view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = OrderStatus.Processed;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<OrderCancelled> eventEnvelope, OrderDetails view)
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

    public static void Handle(EventEnvelope<OrderCompleted> eventEnvelope, OrderDetails view)
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