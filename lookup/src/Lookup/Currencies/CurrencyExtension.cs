using FW.Core.Commands;
using FW.Core.Events;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB.Projections;
using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.Validation;
using Lookup.Currencies.GettingCurrencies;
using Lookup.Currencies.GettingCurrencyHistory;
using Lookup.Currencies.Modifying;
using Lookup.Currencies.Registering;
using Lookup.Currencies.Removing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Lookup.Currencies;

internal static class CurrencyExtension
{
    internal static IServiceCollection AddCurrency(this IServiceCollection services) =>
        services
            .AddScoped<IEventStoreDBRepository<Currency>, EventStoreDBRepository<Currency>>()
            .AddCommandValidators()
            .AddCommandHandlers()
            .AddEventHandlers()
            .AddProjections()
            .AddQueryHandlers();

    private static IServiceCollection AddCommandValidators(this IServiceCollection services) =>
        services
            .AddCommandValidator<RegisterCurrency, ValidateRegisterCurrency>()
            .AddCommandValidator<ModifyCurrency, ValidateModifyCurrency>()
            .AddCommandValidator<RemoveCurrency, ValidateRemoveCurrency>();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<RegisterCurrency, HandleRegisterCurrency>()
            .AddCommandHandler<ModifyCurrency, HandleModifyCurrency>()
            .AddCommandHandler<RemoveCurrency, HandleRemoveCurrency>();

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services
            .AddQueryHandler<GetCurrencies, IListPaged<CurrencyShortInfo>, HandleGetCurrencies>()
            .AddQueryHandler<GetCurrencyList, IListUnpaged<CurrencyShortInfo>, HandleGetCurrencyList>()
            .AddQueryHandler<GetCurrencyById, CurrencyShortInfo, HandleGetCurrencyById>()
            .AddQueryHandler<GetCurrencyHistory, IListPaged<CurrencyHistory>, HandleGetCurrencyHistory>();

    private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services
            .AddEventHandler<EventEnvelope<Publishing.CurrencyRegistered>, Publishing.HandleCurrencyChanges>()
            .AddEventHandler<EventEnvelope<Publishing.CurrencyModified>, Publishing.HandleCurrencyChanges>()
            .AddEventHandler<EventEnvelope<Publishing.CurrencyRemoved>, Publishing.HandleCurrencyChanges>();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services
            .Projection<CurrencyShortInfo>(builder =>
                builder
                    .AddOn<CurrencyRegistered>(CurrencyShortInfoProjection.Handle)
                    .UpdateOn<CurrencyModified>(
                        onGet: e => e.Id,
                        onHandle: CurrencyShortInfoProjection.Handle,
                        onUpdate: (view) => Builders<CurrencyShortInfo>.Update
                            .Set(e => e.Name, view.Name)
                            .Set(e => e.Code, view.Code)
                            .Set(e => e.Symbol, view.Symbol)
                            .Set(e => e.Version, view.Version)
                            .Set(e => e.LastProcessedPosition, view.LastProcessedPosition)
                    )
                    .UpdateOn<CurrencyRemoved>(
                        onGet: e => e.Id,
                        onHandle: CurrencyShortInfoProjection.Handle,
                        onUpdate: (view) => Builders<CurrencyShortInfo>.Update
                            .Set(e => e.Status, view.Status)
                            .Set(e => e.Version, view.Version)
                            .Set(e => e.LastProcessedPosition, view.LastProcessedPosition)
                    )
            )
            .Projection<CurrencyHistory>(builder =>
                builder
                    .AddOn<CurrencyRegistered>(CurrencyHistoryProjection.Handle)
                    .AddOn<CurrencyModified>(CurrencyHistoryProjection.Handle)
                    .AddOn<CurrencyRemoved>(CurrencyHistoryProjection.Handle)
            );
}