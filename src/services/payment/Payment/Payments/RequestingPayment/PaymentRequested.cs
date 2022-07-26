namespace Payment.Payments.RequestingPayment;

public record PaymentRequested(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    DateTime RequestedAt
)
{
    public static PaymentRequested Create(Guid paymentId, Guid orderId, in decimal amount, DateTime requesteAt)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentNullException(nameof(paymentId));
        if (orderId == Guid.Empty)
            throw new ArgumentNullException(nameof(orderId));
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        return new(paymentId, orderId, amount, requesteAt);
    }
}