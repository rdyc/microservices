using FW.Core.Commands;
using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.WebApi;
using Microsoft.AspNetCore.Mvc;
using Shipment.Packages.DiscardingPackage;
using Shipment.Packages.GettingPackageById;
using Shipment.Packages.GettingPackages;
using Shipment.Packages.PreparingPackage;
using Shipment.Packages.SendingPackage;
using Shipment.WebApi.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace Shipment.WebApi.Endpoints;

public static class PackageEndpoint
{
    [SwaggerOperation(Summary = "Retrieve all packages", OperationId = "packages", Tags = new[] { "Package" })]
    internal static async Task<IResult> Packages(
        int index,
        int size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetPackages, IListPaged<PackageShortInfo>>(
            GetPackages.Create(index, size), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Retrieve package", OperationId = "package", Tags = new[] { "Package" })]
    internal static async Task<IResult> Package(
        [FromRoute] Guid packageId,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetPackageById, PackageDetails>(
            GetPackageById.Create(packageId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Prepare a new package", OperationId = "prepare", Tags = new[] { "Package" })]
    internal static async Task<IResult> Prepare(
        [FromBody] PreparePackageRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var packageId = Guid.NewGuid();
        var task = command.SendAsync(
            PreparePackage.Create(packageId, request.OrderId, DateTime.UtcNow),
            cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Created(string.Empty, packageId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Sending package", OperationId = "send", Tags = new[] { "Package" })]
    internal static async Task<IResult> Send(
        [FromRoute] Guid packageId,
        [FromBody] SendPackageRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var (orderId, items) = request;
        var task = command.SendAsync(
            SendPackage.Create(packageId, orderId, items.Select(e => 
                new PackageItem(e.ProductId, e.Quantity)), DateTime.UtcNow),
            cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.NoContent(),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Discard package", OperationId = "discard", Tags = new[] { "Package" })]
    internal static async Task<IResult> Discard(
        [FromRoute] Guid packageId,
        [FromBody] DiscardPackageRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = command.SendAsync(
            DiscardPackage.Create(packageId, request.OrderId, DateTime.UtcNow),
            cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Accepted(string.Empty, packageId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }
}