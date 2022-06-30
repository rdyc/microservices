using FW.Core.MongoDB.Projections;
using Lookup.Currencies.Modifying;
using Lookup.Currencies.Registering;
using Lookup.Currencies.Removing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Validator.Currencies;

internal static class CurrencyExtension
{
    public static IServiceCollection AddCurrency(this IServiceCollection services) =>
        services
            .AddProjections();

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
                    /* multiple updates will not work!, it always use the last singleton service from "prepare" action.

                     .UpdateOn<CurrencyRemoved>(
                        getViewId: e => e.Id,
                        handler: CurrencyShortInfoProjection.Handle,
                        prepare: (view) => Builders<CurrencyShortInfo>.Update
                            .Set(e => e.Status, view.Status)
                            .Set(e => e.Version, view.Version)
                            .Set(e => e.LastProcessedPosition, view.LastProcessedPosition)
                    ) */
            );
}