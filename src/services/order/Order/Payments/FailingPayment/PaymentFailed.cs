namespace Order.Payments.FailingPayment;

public record PaymentFailed(
    Guid OrderId,
    Guid PaymentId,
    decimal Amount,
    DateTime FailedAt
);