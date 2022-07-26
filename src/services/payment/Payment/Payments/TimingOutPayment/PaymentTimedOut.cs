namespace Payment.Payments.TimingOutPayment;

public record PaymentTimedOut(
    Guid PaymentId,
    DateTime TimedOutAt
)
{
    public static PaymentTimedOut Create(Guid paymentId, in DateTime timedOutAt)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentNullException(nameof(paymentId));
        if (timedOutAt == default)
            throw new InvalidOperationException(nameof(timedOutAt));

        return new(paymentId, timedOutAt);
    }
}