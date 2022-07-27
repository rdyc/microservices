using EventStore.Client;
using FW.Core.EventStoreDB;
using FW.Core.EventStoreDB.Subscriptions;
using FW.Core.MongoDB;
using FW.Core.Validation;
using MediatR;
using MediatR.Pipeline;
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
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>))
            .AddScoped(typeof(IRequestPreProcessor<>), typeof(GenericRequestPreProcessor<>))
            .AddScoped(typeof(IRequestPostProcessor<,>), typeof(GenericRequestPostProcessor<,>))
            .AddMongoDb(configuration)
            .AddEventStoreDB(configuration)
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
    public string? ShipmentsUrl { get; set; }
}