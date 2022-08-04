using EventStore.Client;
using FW.Core.EventStoreDB;
using FW.Core.EventStoreDB.Subscriptions;
using Lookup.Currencies;
using Microsoft.Extensions.DependencyInjection;
using Lookup.Attributes;
using Lookup.Histories;

namespace Lookup;

public static class LookupServices
{
    public static IServiceCollection AddLookupServices(this IServiceCollection services) =>
        services
            .AddEventStoreDBSubscriptionToAll(new EventStoreDBSubscriptionToAllOptions
            {
                SubscriptionId = "lookup",
                // FilterOptions = new(EventTypeFilter.RegularExpression(@"Attribute|Currency"))
                FilterOptions = new(EventTypeFilter.Prefix(
                    "AttributeRegistered", 
                    "AttributeModified", 
                    "AttributeRemoved", 
                    "CurrencyRegistered",
                    "CurrencyModified",
                    "CurrencyRemoved"
                ))
            })
            .AddCurrency()
            .AddAttribute()
            .AddHistory();
}