using Lookup.Currencies.Registering;
using Lookup.WebApi.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Lookup.WebApi.Endpoints;

public static class CurrencyEndpoint
{
    [SwaggerOperation(Summary = "Register a new currency", OperationId = "post", Tags = new[] { "Currency" })]
    internal static async Task<IResult> PostAsync([FromBody]CurrencyCreateRequest request, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var (Name, Code, Symbol) = request;
            var result = await mediator.Send(new RegisterCurrency(Name, Code, Symbol), cancellationToken);

            return await Task.FromResult(Results.Ok(result));
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }
}