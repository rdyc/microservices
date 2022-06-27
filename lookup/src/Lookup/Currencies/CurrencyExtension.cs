using Lookup.Currencies.Registering;
using FW.Core.Events;
using FW.Core.EventStoreDB.Repository;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Lookup.Currencies.Modifying;
using Lookup.Currencies.Removing;
using Lookup.Currencies.GettingCurrencies;

namespace Lookup.Currencies;

internal static class CurrencyExtension
{
    internal static IServiceCollection AddCurrency(this IServiceCollection services) =>
        services
            .AddScoped<IEventStoreDBRepository<Currency>, EventStoreDBRepository<Currency>>()
            .AddCommandHandlers()
            // .AddQueryHandlers()
            .AddEventHandlers();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddTransient<IRequestHandler<RegisterCurrency, Guid>, HandleRegisterCurrency>()
            .AddTransient<IRequestHandler<ModifyCurrency, Guid>, HandleModifyCurrency>()
            .AddTransient<IRequestHandler<RemoveCurrency, Guid>, HandleRemoveCurrency>();

    /* private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services.AddQueryHandler<GetCartById, ShoppingCartDetails?, HandleGetCartById>()
            .AddQueryHandler<GetCartAtVersion, ShoppingCartDetails, HandleGetCartAtVersion>()
            .AddQueryHandler<GetCarts, IPagedList<ShoppingCartShortInfo>, HandleGetCarts>()
            .AddQueryHandler<GetCartHistory, IPagedList<ShoppingCartHistory>, HandleGetCartHistory>(); */

    /* private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services.AddEventHandler<EventEnvelope<ShoppingCartConfirmed>, HandleCartFinalized>(); */

    private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services
            .AddTransient<IEventHandler<CurrencyRegistered>, HandleCurrencyRegistered>()
            .AddTransient<IEventHandler<EventEnvelope<CurrencyRegistered>>, HandleCurrencyRegistered>();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services.AddOn<CurrencyRegistered>(CurrencyShortInfoProjection.Handle);

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