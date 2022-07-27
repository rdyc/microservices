namespace Order.Payments.FinalizingPayment;

public record PaymentFinalized(
    Guid OrderId,
    Guid PaymentId,
    decimal Amount,
    DateTime FinalizedAt
);