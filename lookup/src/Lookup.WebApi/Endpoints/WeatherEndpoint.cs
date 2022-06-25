using Lookup.WebApi.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace Lookup.WebApi.Endpoints;

public static class WeatherEndpoint
{
    [SwaggerOperation(Summary = "Retrieve forecast weather", OperationId = "get", Tags = new[] { "Weather" })]
    internal static async Task<IResult> GetWeathersAsync(ILoggerFactory logger, IWeatherRepository repository, CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        try
        {
            log.LogInformation("retrieving forecast");
            var result = repository.Get();

            return await Task.FromResult(Results.Ok(result));
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            log.LogWarning(ex.Message);
        }

        return Results.NoContent();
    }
}