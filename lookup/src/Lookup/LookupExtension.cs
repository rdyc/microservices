using EventStore.Client;
using FW.Core.EventStoreDB;
using FW.Core.EventStoreDB.Subscriptions;
using FW.Core.Validation;
using FW.Core.MongoDB;
using Lookup.Currencies;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lookup;

public static class LookupExtension
{
    public static IServiceCollection AddLookup(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>))
            .AddScoped(typeof(IRequestPreProcessor<>), typeof(GenericRequestPreProcessor<>))
            .AddScoped(typeof(IRequestPostProcessor<,>), typeof(GenericRequestPostProcessor<,>))
            .AddMongoDb(configuration)
            .AddEventStoreDB(configuration)
            .AddEventStoreDBSubscriptionToAll(new EventStoreDBSubscriptionToAllOptions
            {
                SubscriptionId = "lookup-currency",
                FilterOptions = new(EventTypeFilter.Prefix("Lookup_Currencies_"))
            })
            .AddCurrency();
}