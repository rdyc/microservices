namespace Order.Orders.CancellingOrder;

public record OrderCancelled(
    Guid OrderId,
    Guid? PaymentId,
    OrderCancellationReason OrderCancellationReason,
    DateTime CancelledAt
)
{
    public static OrderCancelled Create(
        Guid orderId,
        Guid? paymentId,
        OrderCancellationReason orderCancellationReason,
        DateTime cancelledAt)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));
        if (cancelledAt == default)
            throw new ArgumentOutOfRangeException(nameof(cancelledAt));

        return new(orderId, paymentId, orderCancellationReason, cancelledAt);
    }
}