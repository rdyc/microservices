namespace Order.Payments.TimingOutPayment;

public record PaymentTimedOut(
    Guid PaymentId,
    Guid OrderId,
    DateTime TimedOutAt
);