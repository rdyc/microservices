using FW.Core.Commands;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB.Projections;
using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.Validation;
using Lookup.Currencies.GettingCurrencies;
using Lookup.Currencies.GettingCurrencyById;
using Lookup.Currencies.GettingCurrencyList;
using Lookup.Currencies.ModifyingCurrency;
using Lookup.Currencies.RegisteringCurrency;
using Lookup.Currencies.RemovingCurrency;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Lookup.Currencies;

internal static class CurrencyServices
{
    internal static IServiceCollection AddCurrency(this IServiceCollection services) =>
        services
            .AddScoped<IEventStoreDBRepository<Currency>, EventStoreDBRepository<Currency>>()
            .AddCommandValidators()
            .AddCommandHandlers()
            .AddQueryHandlers()
            .AddProjections();

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
            .AddQueryHandler<GetCurrencyById, CurrencyShortInfo, HandleGetCurrencyById>();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services
            .Projection<CurrencyShortInfo>(builder => builder
                .AddOn<CurrencyRegistered>(CurrencyShortInfoProjection.Handle)
                .UpdateOn<CurrencyModified>(
                    onGet: e => e.CurrencyId,
                    onHandle: CurrencyShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Name, view.Name)
                        .Set(e => e.Code, view.Code)
                        .Set(e => e.Symbol, view.Symbol)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<CurrencyRemoved>(
                    onGet: e => e.CurrencyId,
                    onHandle: CurrencyShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
        );
}