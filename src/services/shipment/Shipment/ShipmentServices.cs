using EventStore.Client;
using FW.Core.EventStoreDB;
using FW.Core.EventStoreDB.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using Shipment.Orders;
using Shipment.Packages;
using Shipment.Products;

namespace Shipment;

public static class ShipmentServices
{
    public static IServiceCollection AddShipmentServices(this IServiceCollection services) =>
        services
            .AddEventStoreDBSubscriptionToAll(new EventStoreDBSubscriptionToAllOptions
            {
                SubscriptionId = "shipment",
                FilterOptions = new(EventTypeFilter.RegularExpression(@"Package"))
            })
            .AddProduct()
            .AddOrder()
            .AddPackage();
}