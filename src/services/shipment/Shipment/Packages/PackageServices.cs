using FW.Core.Commands;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB.Projections;
using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.Validation;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Shipment.Packages.DiscardingPackage;
using Shipment.Packages.GettingPackageById;
using Shipment.Packages.GettingPackages;
using Shipment.Packages.PreparingPackage;
using Shipment.Packages.RequestingPackage;
using Shipment.Packages.SendingPackage;

namespace Shipment.Packages;

public static class PackageServices
{
    public static IServiceCollection AddPackage(this IServiceCollection services) =>
        services
            .AddScoped<IEventStoreDBRepository<Package>, EventStoreDBRepository<Package>>()
            .AddCommandValidators()
            .AddCommandHandlers()
            .AddQueryHandlers()
            .AddProjections();

    private static IServiceCollection AddCommandValidators(this IServiceCollection services) =>
        services
            .AddCommandValidator<PreparePackage, ValidatePreparePackage>()
            .AddCommandValidator<SendPackage, ValidateSendPackage>()
            .AddCommandValidator<DiscardPackage, ValidateDiscardPackage>();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<PreparePackage, HandlePreparePackage>()
            .AddCommandHandler<SendPackage, HandleSendPackage>()
            .AddCommandHandler<DiscardPackage, HandleDiscardPackage>();

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services
            .AddQueryHandler<GetPackageById, PackageDetails, HandleGetPackageById>()
            .AddQueryHandler<GetPackages, IListPaged<PackageShortInfo>, HandleGetPackages>();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services
            .Projection<PackageShortInfo>(builder => builder
                .AddOn<PackagePrepared>(PackageShortInfoProjection.Handle)
                .UpdateOn<PackageWasSent>(
                    onGet: e => e.OrderId,
                    onHandle: PackageShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductWasOutOfStock>(
                    onGet: e => e.OrderId,
                    onHandle: PackageShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            )
            .Projection<PackageDetails>(builder => builder
                .AddOn<PackagePrepared>(PackageDetailsProjection.Handle)
                .UpdateOn<PackageWasSent>(
                    onGet: e => e.OrderId,
                    onHandle: PackageDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.SentAt, view.SentAt)
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductWasOutOfStock>(
                    onGet: e => e.OrderId,
                    onHandle: PackageDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.CheckedAt, view.CheckedAt)
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            );
}