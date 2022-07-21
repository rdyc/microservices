using EventStore.Client;
using FW.Core.EventStoreDB;
using FW.Core.EventStoreDB.Subscriptions;
using FW.Core.MongoDB;
using FW.Core.Validation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Payment;

public static class PaymentServices
{
    public static IServiceCollection AddPaymentServices(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>))
            .AddScoped(typeof(IRequestPreProcessor<>), typeof(GenericRequestPreProcessor<>))
            .AddScoped(typeof(IRequestPostProcessor<,>), typeof(GenericRequestPostProcessor<,>))
            .AddMongoDb(configuration)
            .AddEventStoreDB(configuration)
            .AddEventStoreDBSubscriptionToAll(new EventStoreDBSubscriptionToAllOptions
            {
                SubscriptionId = "payment",
                FilterOptions = new(EventTypeFilter.RegularExpression(@"Payment"))
            });
}
