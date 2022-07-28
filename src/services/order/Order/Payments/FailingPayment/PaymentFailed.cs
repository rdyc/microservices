namespace Order.Payments.FailingPayment;

public record PaymentFailed(
    Guid OrderId,
    Guid? PaymentId,
    DateTime FailedAt,
    PaymentFailReason FailReason
);

public enum PaymentFailReason
{
    Discarded,
    TimedOut
}