using FW.Core.Commands;
using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.WebApi;
using FW.Core.WebApi.Headers;
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

internal static class CurrencyEndpoint
{
    [SwaggerOperation(Summary = "Retrieve all currencies", OperationId = "currencies", Tags = new[] { "Currency" })]
    internal static async Task<IResult> Currencies(
        [FromQuery] int? page,
        [FromQuery] int? size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken
    )
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetCurrencies, IListPaged<CurrencyShortInfo>>(
            GetCurrencies.Create(page, size), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Retrieve a currency", OperationId = "currency", Tags = new[] { "Currency" })]
    internal static async Task<IResult> Currency(
        [FromRoute] Guid currencyId,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        HttpContext context,
        CancellationToken cancellationToken
    )
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetCurrencyById, CurrencyShortInfo>(
            GetCurrencyById.Create(currencyId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => 
            {
                context.TrySetETagResponseHeader(result.Version);

                return Results.Ok(result);
            },
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Retrieve currency list", OperationId = "currency_list", Tags = new[] { "Currency" })]
    internal static async Task<IResult> CurrencyList(
        [FromQuery] LookupStatus? status,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken
    )
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetCurrencyList, IListUnpaged<CurrencyShortInfo>>(
            GetCurrencyList.Create(status), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Register a new currency", OperationId = "register", Tags = new[] { "Currency" })]
    internal static async Task<IResult> Create(
        [FromBody] CurrencyCreateRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken
    )
    {
        var log = logger.CreateLogger<Program>();
        var currencyId = Guid.NewGuid();
        var (name, code, symbol, status) = request;
        var task = command.SendAsync(
            RegisterCurrency.Create(currencyId, name, code, symbol, status), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Created(string.Empty, currencyId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Modify existing currency", OperationId = "modify", Tags = new[] { "Currency" })]
    internal static async Task<IResult> Update(
        [FromRoute] Guid currencyId,
        [FromBody] CurrencyModifyRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken
    )
    {
        var log = logger.CreateLogger<Program>();
        var (name, code, symbol) = request;
        var task = command.SendAsync(
            ModifyCurrency.Create(currencyId, name, code, symbol), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Accepted(string.Empty, currencyId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Remove existing currency", OperationId = "remove", Tags = new[] { "Currency" })]
    internal static async Task<IResult> Delete(
        [FromRoute] Guid currencyId,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken
    )
    {
        var log = logger.CreateLogger<Program>();
        var task = command.SendAsync(
            RemoveCurrency.Create(currencyId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.NoContent(),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }
}