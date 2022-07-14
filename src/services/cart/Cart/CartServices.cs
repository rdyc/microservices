using Carts.ShoppingCarts;
using FW.Core.EventStoreDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cart;

public static class CartServices
{
    public static IServiceCollection AddCartServices(this IServiceCollection services, IConfiguration config) =>
        services
            .AddCarts()
            .AddEventStoreDB(config);
}
