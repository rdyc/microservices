using FW.Core.Queries;
using FW.Core.WebApi;
using Microsoft.AspNetCore.Mvc;
using Search.Products;
using Search.Products.SearchingProducts;
using Swashbuckle.AspNetCore.Annotations;

namespace Search.WebApi.Endpoints;

internal static class SearchEndpoint
{
    [SwaggerOperation(Summary = "Search products", OperationId = "search", Tags = new[] { "Search" })]
    internal static async Task<IResult> Products(
        [FromQuery] string? find,
        [FromQuery] int? size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<SearchProducts, IReadOnlyCollection<Product>>(
            SearchProducts.Create(find, size), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }
}