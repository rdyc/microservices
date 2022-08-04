using EventStore.Client;
using FW.Core.EventStoreDB;
using FW.Core.EventStoreDB.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using Payment.Payments;

namespace Payment;

public static class PaymentServices
{
    public static IServiceCollection AddPaymentServices(this IServiceCollection services) =>
        services
            .AddEventStoreDBSubscriptionToAll(new EventStoreDBSubscriptionToAllOptions
            {
                SubscriptionId = "payment",
                FilterOptions = new(EventTypeFilter.RegularExpression(@"Payment"))
            })
            .AddPayment();
}