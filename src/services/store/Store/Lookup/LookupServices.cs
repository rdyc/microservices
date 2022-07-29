using FW.Core.MongoDB.Projections;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Store.Lookup.Attributes;
using Store.Lookup.Attributes.ModifyingAttribute;
using Store.Lookup.Attributes.RegisteringAttribute;
using Store.Lookup.Attributes.RemovingAttribute;
using Store.Lookup.Currencies;
using Store.Lookup.Currencies.ModifyingCurrency;
using Store.Lookup.Currencies.RegisteringCurrency;
using Store.Lookup.Currencies.RemovingCurrency;
using Attribute = Store.Lookup.Attributes.Attribute;

namespace Store.Lookup;

internal static class LookupServices
{
    public static IServiceCollection AddLookup(this IServiceCollection services) =>
        services.AddProjections();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services
            .Projection<Attribute>(builder => builder
                .AddOn<AttributeRegistered>(AttributeProjection.Handle)
                .UpdateOn<AttributeModified>(
                    onGet: e => e.AttributeId,
                    onHandle: AttributeProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Name, view.Name)
                        .Set(e => e.Type, view.Type)
                        .Set(e => e.Unit, view.Unit)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<AttributeRemoved>(
                    onGet: e => e.AttributeId,
                    onHandle: AttributeProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            )
            .Projection<Currency>(builder => builder
                .AddOn<CurrencyRegistered>(CurrencyProjection.Handle)
                .UpdateOn<CurrencyModified>(
                    onGet: e => e.CurrencyId,
                    onHandle: CurrencyProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Name, view.Name)
                        .Set(e => e.Code, view.Code)
                        .Set(e => e.Symbol, view.Symbol)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<CurrencyRemoved>(
                    onGet: e => e.CurrencyId,
                    onHandle: CurrencyProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            );
}