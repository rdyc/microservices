using FW.Core.Commands;
using Microsoft.Extensions.DependencyInjection;
using Order.Payments.DiscardingPayment;
using Order.Payments.RequestingPayment;

namespace Order.Payments;

internal static class PaymentsConfig
{
    internal static IServiceCollection AddPayment(this IServiceCollection services) =>
        services
            .AddCommandHandler<DiscardPayment, HandleDiscardPayment>();
            // .AddCommandHandler<RequestPayment, HandleRequestPayment>();
}