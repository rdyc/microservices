using FW.Core.Commands;
using FW.Core.Pagination;
using FW.Core.Queries;
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

        try
        {
            var result = await query.SendAsync<GetAttributes, IListPaged<AttributeShortInfo>>(
                new GetAttributes(index, size), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Retrieve attribute", OperationId = "get_detail", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> Attribute(
        Guid id,
        IQueryBus query,
        ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await query.SendAsync<GetAttributeById, AttributeShortInfo>(
                new GetAttributeById(id), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Retrieve attribute list", OperationId = "get_list", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> AttributeList(
        [FromQuery] LookupStatus? status,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await query.SendAsync<GetAttributeList, IListUnpaged<AttributeShortInfo>>(
                new GetAttributeList(status), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Register a new attribute", OperationId = "post", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> Create(
        [FromBody] AttributeCreateRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var id = Guid.NewGuid();
            var (name, code, symbol, status) = request;

            await command.SendAsync(new RegisterAttribute(id, name, code, symbol, status), cancellationToken);

            return Results.Created(string.Empty, id);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
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

        try
        {
            var (name, code, symbol) = request;

            await command.SendAsync(new ModifyAttribute(attributeId, name, code, symbol), cancellationToken);

            return Results.Accepted(string.Empty, attributeId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Remove existing attribute", OperationId = "delete", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> Delete(
        [FromRoute] Guid attributeId,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await command.SendAsync(new RemoveAttribute(attributeId), cancellationToken);

            return Results.NoContent();
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }
}