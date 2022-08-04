using EventStore.Client;
using FW.Core.EventStoreDB;
using FW.Core.EventStoreDB.Subscriptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Orders;
using Order.Payments;

namespace Order;

public static class OrderServices
{
    public static IServiceCollection AddOrderServices(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddSingleton(sp => configuration.GetSection(ExternalServicesConfig.ConfigName).Get<ExternalServicesConfig>())
            .AddEventStoreDBSubscriptionToAll(new EventStoreDBSubscriptionToAllOptions
            {
                SubscriptionId = "order",
                FilterOptions = new(EventTypeFilter.RegularExpression(@"Order"))
            })
            .AddPayment()
            .AddOrder();
}

public class ExternalServicesConfig
{
    public static string ConfigName = "ExternalServices";
    public string? PaymentsUrl { get; set; }
}