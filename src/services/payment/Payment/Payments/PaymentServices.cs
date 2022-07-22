using FW.Core.Commands;
using FW.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Payment.Payments.CompletingPayment;
using Payment.Payments.DiscardingPayment;
using Payment.Payments.FailingPayment;
using Payment.Payments.FinalizingPayment;
using Payment.Payments.RequestingPayment;
using Payment.Payments.TimingOutPayment;

namespace Payment.Payments;

internal static class PaymentServices
{
    internal static IServiceCollection AddPayment(this IServiceCollection services) =>
        services
            .AddCommandHandlers()
            .AddEventHandlers();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<RequestPayment, HandleRequestPayment>()
            .AddCommandHandler<CompletePayment, HandleCompletePayment>()
            .AddCommandHandler<DiscardPayment, HandleDiscardPayment>()
            .AddCommandHandler<TimeOutPayment, HandleTimeOutPayment>();

    private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services
            .AddEventHandler<EventEnvelope<PaymentCompleted>, TransformIntoPaymentFinalized>()
            .AddEventHandler<EventEnvelope<PaymentDiscarded>, TransformIntoPaymentFailed>()
            .AddEventHandler<EventEnvelope<PaymentTimedOut>, TransformIntoPaymentFailed>();
}