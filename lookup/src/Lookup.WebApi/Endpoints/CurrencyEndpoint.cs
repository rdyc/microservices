using Lookup.Currencies.GettingCurrencies;
using Lookup.Currencies.GettingCurrencyHistory;
using Lookup.Currencies.ModifyingCurrency;
using Lookup.Currencies.RegisteringCurrency;
using Lookup.Currencies.RemovingCurrency;
using Lookup.WebApi.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Lookup.WebApi.Endpoints;

public static class CurrencyEndpoint
{
    [SwaggerOperation(Summary = "Retrieve all currencies", OperationId = "get_all", Tags = new[] { "Currency" })]
    internal static async Task<IResult> GetAllAsync(int index, int size, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
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

    [SwaggerOperation(Summary = "Retrieve currency", OperationId = "get_detail", Tags = new[] { "Currency" })]
    internal static async Task<IResult> GetDetailAsync(Guid id, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await mediator.Send(new GetCurrencyById(id), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Retrieve currency list", OperationId = "get_list", Tags = new[] { "Currency" })]
    internal static async Task<IResult> GetListAsync(LookupStatus? status, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await mediator.Send(new GetCurrencyList(status), cancellationToken);

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
            var id = Guid.NewGuid();
            var (name, code, symbol, status) = request;

            await mediator.Send(new RegisterCurrency(id, name, code, symbol, status), cancellationToken);

            return Results.Created(string.Empty, id);
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

            await mediator.Send(new ModifyCurrency(id, name, code, symbol), cancellationToken);

            return Results.Accepted(string.Empty, id);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Remove existing currency", OperationId = "delete", Tags = new[] { "Currency" })]
    internal static async Task<IResult> DeleteAsync(Guid id, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await mediator.Send(new RemoveCurrency(id), cancellationToken);

            return Results.NoContent();
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }
}