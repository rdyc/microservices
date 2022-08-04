using EventStore.Client;
using FW.Core.EventStoreDB;
using FW.Core.EventStoreDB.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using Store.Products;
using Store.Lookup;

namespace Store;

public static class StoreServices
{
    public static IServiceCollection AddStoreServices(this IServiceCollection services) =>
        services
            .AddEventStoreDBSubscriptionToAll(new EventStoreDBSubscriptionToAllOptions
            {
                SubscriptionId = "store",
                FilterOptions = new(EventTypeFilter.Prefix("Product"))
            })
            .AddLookup()
            .AddProduct();
}