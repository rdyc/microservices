using FW.Core.Events;
using Order.ShoppingCarts.FinalizingCart;

namespace Order.Orders.RecordingOrderPayment;

public record OrderPaymentRecorded(
    Guid OrderId,
    Guid PaymentId,
    IEnumerable<ShoppingCartProduct> Products,
    decimal TotalPrice,
    DateTime RecordedAt
) : IExternalEvent
{
    public static OrderPaymentRecorded Create(
        Guid orderId,
        Guid paymentId,
        IEnumerable<ShoppingCartProduct> products,
        decimal totalPrice,
        DateTime recordedAt)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));
        if (products is null || !products.Any())
            throw new ArgumentOutOfRangeException(nameof(products));
        if (totalPrice > 0)
            throw new ArgumentOutOfRangeException(nameof(totalPrice));
        if (recordedAt == default)
            throw new ArgumentOutOfRangeException(nameof(recordedAt));

        return new OrderPaymentRecorded(
            orderId,
            paymentId,
            products,
            totalPrice,
            recordedAt
        );
    }
}