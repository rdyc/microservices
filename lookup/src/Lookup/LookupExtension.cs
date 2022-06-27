using EventStore.Client;
using FW.Core.EventStoreDB;
using FW.Core.EventStoreDB.Subscriptions;
using Lookup.Currencies;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lookup;

public static class LookupExtension
{
    public static IServiceCollection AddLookup(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddEventStoreDB(configuration)
            .AddEventStoreDBSubscriptionToAll(new EventStoreDBSubscriptionToAllOptions
            {
                SubscriptionId = "lookup-currency",
                FilterOptions = new(EventTypeFilter.Prefix("Lookup_Currencies_"))
            })
            .AddCurrency()
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
}