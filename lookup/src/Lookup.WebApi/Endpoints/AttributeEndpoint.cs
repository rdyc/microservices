using Lookup.Attributes.GettingAttributeHistory;
using Lookup.Attributes.GettingAttributes;
using Lookup.Attributes.ModifyingAttribute;
using Lookup.Attributes.RegisteringAttribute;
using Lookup.Attributes.RemovingAttribute;
using Lookup.WebApi.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Lookup.WebApi.Endpoints;

public static class AttributeEndpoint
{
    [SwaggerOperation(Summary = "Retrieve all attributes", OperationId = "get_all", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> GetAllAsync(int index, int size, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await mediator.Send(new GetAttributes(index, size), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Retrieve attribute", OperationId = "get_detail", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> GetDetailAsync(Guid id, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await mediator.Send(new GetAttributeById(id), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Retrieve attribute list", OperationId = "get_list", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> GetListAsync(LookupStatus? status, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await mediator.Send(new GetAttributeList(status), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Register a new attribute", OperationId = "post", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> PostAsync([FromBody] AttributeCreateRequest request, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var id = Guid.NewGuid();
            var (name, code, symbol, status) = request;

            await mediator.Send(new RegisterAttribute(id, name, code, symbol, status), cancellationToken);

            return Results.Created(string.Empty, id);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Modify existing attribute", OperationId = "put", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> PutAsync(Guid id, [FromBody] AttributeModifyRequest request, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var (name, code, symbol) = request;

            await mediator.Send(new ModifyAttribute(id, name, code, symbol), cancellationToken);

            return Results.Accepted(string.Empty, id);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Remove existing attribute", OperationId = "delete", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> DeleteAsync(Guid id, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await mediator.Send(new RemoveAttribute(id), cancellationToken);

            return Results.NoContent();
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }
}