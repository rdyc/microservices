using Cart.Products;
using Cart.Carts;
using EventStore.Client;
using FW.Core.EventStoreDB;
using FW.Core.EventStoreDB.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace Cart;

public static class CartServices
{
    public static IServiceCollection AddCartServices(this IServiceCollection services) =>
        services
            .AddEventStoreDBSubscriptionToAll(new EventStoreDBSubscriptionToAllOptions
            {
                SubscriptionId = "cart",
                FilterOptions = new(EventTypeFilter.RegularExpression(@"Cart"))
            })
            .AddProduct()
            .AddCart();
}
