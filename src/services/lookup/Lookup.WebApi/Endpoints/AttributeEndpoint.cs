using FW.Core.Commands;
using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.WebApi;
using Lookup.Attributes.GettingAttributeById;
using Lookup.Attributes.GettingAttributeList;
using Lookup.Attributes.GettingAttributes;
using Lookup.Attributes.ModifyingAttribute;
using Lookup.Attributes.RegisteringAttribute;
using Lookup.Attributes.RemovingAttribute;
using Lookup.WebApi.Requests;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Lookup.WebApi.Endpoints;

public static class AttributeEndpoint
{
    [SwaggerOperation(Summary = "Retrieve all attributes", OperationId = "get_all", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> Attributes(
        int index,
        int size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetAttributes, IListPaged<AttributeShortInfo>>(
            GetAttributes.Create(index, size), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Retrieve attribute", OperationId = "get_detail", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> Attribute(
        [FromRoute] Guid attributeId,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetAttributeById, AttributeShortInfo>(
            GetAttributeById.Create(attributeId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Retrieve attribute list", OperationId = "get_list", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> AttributeList(
        [FromQuery] LookupStatus? status,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetAttributeList, IListUnpaged<AttributeShortInfo>>(
            GetAttributeList.Create(status), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Register a new attribute", OperationId = "post", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> Create(
        [FromBody] AttributeCreateRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var attributeId = Guid.NewGuid();
        var (name, code, symbol, status) = request;
        var task = command.SendAsync(RegisterAttribute.Create(attributeId, name, code, symbol, status), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Created(string.Empty, attributeId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Modify existing attribute", OperationId = "put", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> Update(
        [FromRoute] Guid attributeId,
        [FromBody] AttributeModifyRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var (name, code, symbol) = request;
        var task = command.SendAsync(ModifyAttribute.Create(attributeId, name, code, symbol), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Accepted(string.Empty, attributeId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Remove existing attribute", OperationId = "delete", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> Delete(
        [FromRoute] Guid attributeId,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = command.SendAsync(RemoveAttribute.Create(attributeId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.NoContent(),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }
}