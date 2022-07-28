namespace Order.Payments.FinalizingPayment;

public record PaymentFinalized(
    Guid PaymentId,
    Guid OrderId,
    DateTime FinalizedAt
);