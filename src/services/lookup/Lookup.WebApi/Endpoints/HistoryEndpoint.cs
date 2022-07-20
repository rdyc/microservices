using FW.Core.Pagination;
using FW.Core.Queries;
using Lookup.Histories.GettingHistories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Lookup.WebApi.Endpoints;

public static class HistoryEndpoint
{
    [SwaggerOperation(Summary = "Retrieve lookup histories", OperationId = "get", Tags = new[] { "History" })]
    internal static async Task<IResult> Histories(
        [FromRoute] Guid aggregateid,
        int index,
        int size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await query.SendAsync<GetHistory, IListPaged<History>>(
                new GetHistory(aggregateid, index, size), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }
}