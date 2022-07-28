namespace Payment.Payments.CompletingPayment;

public record PaymentCompleted(
    Guid PaymentId,
    Guid OrderId,
    DateTime CompletedAt
)
{
    public static PaymentCompleted Create(Guid paymentId, Guid orderId, DateTime completedAt)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));
        if (completedAt == default)
            throw new ArgumentOutOfRangeException(nameof(completedAt));

        return new(paymentId, orderId, completedAt);
    }
}