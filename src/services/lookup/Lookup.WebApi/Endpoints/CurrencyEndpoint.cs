using FW.Core.Commands;
using FW.Core.Pagination;
using FW.Core.Queries;
using Lookup.Currencies.GettingCurrencies;
using Lookup.Currencies.GettingCurrencyById;
using Lookup.Currencies.GettingCurrencyList;
using Lookup.Currencies.ModifyingCurrency;
using Lookup.Currencies.RegisteringCurrency;
using Lookup.Currencies.RemovingCurrency;
using Lookup.WebApi.Requests;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Lookup.WebApi.Endpoints;

public static class CurrencyEndpoint
{
    [SwaggerOperation(Summary = "Retrieve all currencies", OperationId = "get_all", Tags = new[] { "Currency" })]
    internal static async Task<IResult> Currencies(
        int index,
        int size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await query.SendAsync<GetCurrencies, IListPaged<CurrencyShortInfo>>(
                new GetCurrencies(index, size), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Retrieve currency", OperationId = "get_detail", Tags = new[] { "Currency" })]
    internal static async Task<IResult> Currency(
        [FromRoute] Guid currencyId,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await query.SendAsync<GetCurrencyById, CurrencyShortInfo>(
                new GetCurrencyById(currencyId), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Retrieve currency list", OperationId = "get_list", Tags = new[] { "Currency" })]
    internal static async Task<IResult> CurrencyList(
        [FromQuery] LookupStatus? status,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await query.SendAsync<GetCurrencyList, IListUnpaged<CurrencyShortInfo>>(
                new GetCurrencyList(status), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Register a new currency", OperationId = "post", Tags = new[] { "Currency" })]
    internal static async Task<IResult> Create(
        [FromBody] CurrencyCreateRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var id = Guid.NewGuid();
            var (name, code, symbol, status) = request;

            await command.SendAsync(new RegisterCurrency(id, name, code, symbol, status), cancellationToken);

            return Results.Created(string.Empty, id);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Modify existing currency", OperationId = "put", Tags = new[] { "Currency" })]
    internal static async Task<IResult> Update(
        [FromRoute] Guid currencyId,
        [FromBody] CurrencyModifyRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var (name, code, symbol) = request;

            await command.SendAsync(new ModifyCurrency(currencyId, name, code, symbol), cancellationToken);

            return Results.Accepted(string.Empty, currencyId);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }

    [SwaggerOperation(Summary = "Remove existing currency", OperationId = "delete", Tags = new[] { "Currency" })]
    internal static async Task<IResult> Delete(
        [FromRoute] Guid currencyId,
        [FromServices] ICommandBus mediator,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            await mediator.SendAsync(new RemoveCurrency(currencyId), cancellationToken);

            return Results.NoContent();
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }
}