using Payment.Payments.DiscardingPayment;

namespace Payment.WebApi.Requests;

public record CreatePaymentRequest(
    Guid OrderId,
    decimal Amount
);

public record DiscardPaymentRequest(
    DiscardReason Reason
);