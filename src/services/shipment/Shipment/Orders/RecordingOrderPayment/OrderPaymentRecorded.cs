using Shipment.Carts;

namespace Shipment.Orders.RecordingOrderPayment;

public record OrderPaymentRecorded(
    Guid ClientId,
    Guid OrderId,
    Guid PaymentId,
    IEnumerable<CartProduct> Products,
    decimal TotalPrice,
    DateTime RecordedAt
);