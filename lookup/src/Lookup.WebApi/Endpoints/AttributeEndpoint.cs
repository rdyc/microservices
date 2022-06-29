using Lookup.Currencies.GettingCurrencies;
using Lookup.Currencies.Modifying;
using Lookup.Currencies.Registering;
using Lookup.WebApi.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Lookup.WebApi.Endpoints;

public static class AttributeEndpoint
{
    [SwaggerOperation(Summary = "Retrieve all attributes", OperationId = "get_all", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> GetAsync(int index, int size, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await mediator.Send(new GetCurrencies(index, size), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Register a new attribute", OperationId = "post", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> PostAsync([FromBody] CurrencyCreateRequest request, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var (name, code, symbol, status) = request;
            var result = await mediator.Send(new RegisterCurrency(name, code, symbol, status), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Modify existing attribute", OperationId = "put", Tags = new[] { "Attribute" })]
    internal static async Task<IResult> PutAsync(Guid id, [FromBody] CurrencyModifyRequest request, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var (name, code, symbol) = request;
            var result = await mediator.Send(new ModifyCurrency(id, name, code, symbol), cancellationToken);

            return Results.AcceptedAtRoute("get_all_currencies", result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }
}