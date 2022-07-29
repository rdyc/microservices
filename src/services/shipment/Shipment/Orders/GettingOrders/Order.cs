using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using Shipment.Orders.RecordingOrderPayment;

namespace Shipment.Orders.GettingOrders;

[BsonCollection("order")]
public record Order : Document
{
    [BsonElement("client_id")]
    public Guid ClientId { get; set; }

    [BsonElement("products")]
    public IList<OrderProduct> Products { get; set; } = default!;

    [BsonElement("version")]
    public ulong Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
}

public record OrderProduct
{
    public OrderProduct(Guid id, string sku, string name, int quantity)
    {
        Id = id;
        Sku = sku;
        Name = name;
        Quantity = quantity;
    }

    [BsonElement("id")]
    public Guid Id { get; private set; }

    [BsonElement("sku")]
    public string Sku { get; private set; }

    [BsonElement("name")]
    public string Name { get; private set; }

    [BsonElement("quantity")]
    public int Quantity { get; private set; }
}

internal static class OrderProjection
{
    public static Order Handle(EventEnvelope<OrderPaymentRecorded> eventEnvelope)
    {
        var (clientId, orderId, paymentId, products, _, recordedAt) = eventEnvelope.Data;

        return new Order
        {
            Id = orderId,
            ClientId = clientId,
            Products = products
                .Select(e => new OrderProduct(
                    e.ProductId,
                    e.Sku,
                    e.Name,
                    e.Quantity)
                )
                .ToList(),
            Version = eventEnvelope.Metadata.StreamPosition,
            Position = eventEnvelope.Metadata.LogPosition
        };
    }
}