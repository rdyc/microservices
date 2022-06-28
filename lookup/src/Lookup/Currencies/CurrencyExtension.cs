using Lookup.Currencies.Registering;
using FW.Core.Events;
using FW.Core.EventStoreDB.Repository;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Lookup.Currencies.Modifying;
using Lookup.Currencies.Removing;
using Lookup.Currencies.GettingCurrencies;
using FW.Core.MongoDB.Projections;
using FW.Core.MongoDB.Commands;
using MongoDB.Driver;

namespace Lookup.Currencies;

internal static class CurrencyExtension
{
    internal static IServiceCollection AddCurrency(this IServiceCollection services) =>
        services
            .AddScoped<IEventStoreDBRepository<Currency>, EventStoreDBRepository<Currency>>()
            .AddCommandHandlers()
            .AddProjections()
            .AddQueryHandlers();
            // .AddEventHandlers();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddTransient<IRequestHandler<RegisterCurrency, Guid>, HandleRegisterCurrency>()
            .AddTransient<IRequestHandler<ModifyCurrency, Guid>, HandleModifyCurrency>()
            .AddTransient<IRequestHandler<RemoveCurrency, Guid>, HandleRemoveCurrency>();

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services
            .AddTransient<IRequestHandler<GetCurrencies, IEnumerable<CurrencyShortInfo>>, HandleGetCurrencies>();

    /* private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services.AddEventHandler<EventEnvelope<ShoppingCartConfirmed>, HandleCartFinalized>(); */

    /* private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services
            .AddTransient<IEventHandler<CurrencyRegistered>, HandleCurrencyRegistered>()
            .AddTransient<IEventHandler<EventEnvelope<CurrencyRegistered>>, HandleCurrencyRegistered>(); */

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services
            .For<CurrencyShortInfo>(builder => 
                builder
                    .AddOn<CurrencyRegistered>(CurrencyShortInfoProjection.Handle)
                    .UpdateOn<CurrencyModified>(
                        getViewId: e => e.Id, 
                        handler: CurrencyShortInfoProjection.Handle,
                        prepare: (view) => Builders<CurrencyShortInfo>.Update
                            .Set(e => e.Name, view.Name)
                            .Set(e => e.Code, view.Code)
                            .Set(e => e.Symbol, view.Symbol)
                            .Set(e => e.Version, view.Version)
                            .Set(e => e.LastProcessedPosition, view.LastProcessedPosition)
                    )
            );

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