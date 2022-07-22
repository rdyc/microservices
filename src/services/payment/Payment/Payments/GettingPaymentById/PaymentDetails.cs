using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Payment.Payments.GettingPaymentById;

[BsonCollection("payment_details")]
public record PaymentDetails : Document
{
    [BsonElement("order_id")]
    public Guid OrderId { get; set; } = default!;

    [BsonElement("amount")]
    public decimal Amount { get; set; } = default!;
}