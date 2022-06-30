using EventStore.Client;
using FW.Core.EventStoreDB;
using FW.Core.EventStoreDB.Subscriptions;
using FW.Core.MongoDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Validator.Currencies;

namespace Validator;

public static class ValidatorExtension
{
    public static IServiceCollection AddValidator(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddMongoDb(configuration)
            .AddEventStoreDB(configuration)
            .AddEventStoreDBSubscriptionToAll(new EventStoreDBSubscriptionToAllOptions
            {
                SubscriptionId = "validator-currency",
                FilterOptions = new(EventTypeFilter.Prefix("Lookup_Currencies_"))
            })
            .AddCurrency();
}