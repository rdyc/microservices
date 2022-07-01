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
            .Projection<CurrencyShortInfo>("currency_shortinfo", builder =>
                builder
                    .AddOn<CurrencyRegistered>(CurrencyShortInfoProjection.Handle)
                    .UpdateOn<CurrencyModified>(
                        onHandle: CurrencyShortInfoProjection.Handle,
                        onGet: e => e.Id,
                        onUpdate: (view) => Builders<CurrencyShortInfo>.Update
                            .Set(e => e.Name, view.Name)
                            .Set(e => e.Code, view.Code)
                            .Set(e => e.Symbol, view.Symbol)
                            .Set(e => e.Version, view.Version)
                            .Set(e => e.LastProcessedPosition, view.LastProcessedPosition)
                    )
                     .UpdateOn<CurrencyRemoved>(
                        onHandle: CurrencyShortInfoProjection.Handle,
                        onGet: e => e.Id,
                        onUpdate: (view) => Builders<CurrencyShortInfo>.Update
                            .Set(e => e.Status, view.Status)
                            .Set(e => e.Version, view.Version)
                            .Set(e => e.LastProcessedPosition, view.LastProcessedPosition)
                    ) 
            );
}