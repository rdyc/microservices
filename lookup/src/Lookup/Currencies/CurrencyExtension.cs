using Lookup.Currencies.RegisterCurrency;
using FW.Core.Events;
using FW.Core.EventStoreDB.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Lookup.Currencies;

internal static class CurrencyExtension
{
    internal static IServiceCollection AddCarts(this IServiceCollection services) =>
        services.AddScoped<IEventStoreDBRepository<Currency>, EventStoreDBRepository<Currency>>();
            // .AddCommandHandlers()
            // .AddQueryHandlers()
            // .AddEventHandlers();

    /* private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services.AddTransient<RegisterCurrency, HandleRegisterCurrency>(); */

    /* private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services.AddQueryHandler<GetCartById, ShoppingCartDetails?, HandleGetCartById>()
            .AddQueryHandler<GetCartAtVersion, ShoppingCartDetails, HandleGetCartAtVersion>()
            .AddQueryHandler<GetCarts, IPagedList<ShoppingCartShortInfo>, HandleGetCarts>()
            .AddQueryHandler<GetCartHistory, IPagedList<ShoppingCartHistory>, HandleGetCartHistory>(); */

    /* private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services.AddEventHandler<EventEnvelope<ShoppingCartConfirmed>, HandleCartFinalized>(); */

    /* internal static void ConfigureCarts(this StoreOptions options)
    {
        // Snapshots
        options.Projections.SelfAggregate<ShoppingCart>();
        // // projections
        options.Projections.Add<CartShortInfoProjection>();
        options.Projections.Add<CartDetailsProjection>();
        //
        // // transformation
        options.Projections.Add<CartHistoryTransformation>();
    } */
}