using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB.Projections;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Shipment.Packages;

public static class PackageServices
{
    public static IServiceCollection AddPackage(this IServiceCollection services) =>
        services
            .AddScoped<IEventStoreDBRepository<Package>, EventStoreDBRepository<Package>>()
            .AddProjections();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services;
            /* .Projection<Package>(builder => builder
                .AddOn<PackageRegistered>(PackageProjection.Handle)
                .UpdateOn<PackageModified>(
                    onGet: e => e.Id,
                    onHandle: PackageProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Sku, view.Sku)
                        .Set(e => e.Name, view.Name)
                        .Set(e => e.Description, view.Description)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<PackageStockChanged>(
                    onGet: e => e.Id,
                    onHandle: PackageProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Stock, view.Stock)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<PackageRemoved>(
                    onGet: e => e.Id,
                    onHandle: PackageProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            ); */
}