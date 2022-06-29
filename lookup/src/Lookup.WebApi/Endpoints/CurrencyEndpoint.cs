using Lookup.Currencies.GettingCurrencies;
using Lookup.Currencies.Modifying;
using Lookup.Currencies.Registering;
using Lookup.WebApi.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Lookup.WebApi.Endpoints;

public static class CurrencyEndpoint
{
    [SwaggerOperation(Summary = "Retrieve all currencies", OperationId = "get_all", Tags = new[] { "Currency" })]
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

    [SwaggerOperation(Summary = "Register a new currency", OperationId = "post", Tags = new[] { "Currency" })]
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

    [SwaggerOperation(Summary = "Modify existing currency", OperationId = "put", Tags = new[] { "Currency" })]
    internal static async Task<IResult> PutAsync(Guid id, [FromBody] CurrencyModifyRequest request, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var (name, code, symbol) = request;
            var result = await mediator.Send(new ModifyCurrency(id, name, code, symbol), cancellationToken);

            return Results.AcceptedAtRoute("get_currencies", result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }
}