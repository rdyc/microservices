using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using Lookup.Histories.GettingHistories;

namespace Lookup.WebApi.Endpoints;

public static class HistoryEndpoint
{
    [SwaggerOperation(Summary = "Retrieve lookup histories", OperationId = "get", Tags = new[] { "History" })]
    internal static async Task<IResult> GetHistoryAsync(Guid id, int index, int size, IMediator mediator, ILoggerFactory logger, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();

        try
        {
            var result = await mediator.Send(new GetHistory(id, index, size), cancellationToken);

            return Results.Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }
}