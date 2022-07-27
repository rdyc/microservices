using FW.Core.Events;

namespace Payment.Payments.TimingOutPayment;

public record PaymentTimedOut(
    Guid PaymentId,
    Guid OrderId,
    DateTime TimedOutAt
) : IExternalEvent
{
    public static PaymentTimedOut Create(Guid paymentId, Guid orderId, in DateTime timedOutAt)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentNullException(nameof(paymentId));
        if (timedOutAt == default)
            throw new InvalidOperationException(nameof(timedOutAt));

        return new(paymentId, orderId, timedOutAt);
    }
}