using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.WebApi;
using Lookup.Histories.GettingHistories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Lookup.WebApi.Endpoints;

public static class HistoryEndpoint
{
    [SwaggerOperation(Summary = "Retrieve lookup histories", OperationId = "histories", Tags = new[] { "History" })]
    internal static async Task<IResult> Histories(
        [FromRoute] Guid aggregateId,
        [FromQuery] int? page,
        [FromQuery] int? size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetHistory, IListPaged<History>>(
            GetHistory.Create(aggregateId, page, size), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }
}