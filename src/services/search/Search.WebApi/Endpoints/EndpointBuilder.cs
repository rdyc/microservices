using System.Net;
using Search.Products;

namespace Search.WebApi.Endpoints;

internal static class EndpointBuilder
{
    internal static IApplicationBuilder UseSearchEndpoints(this WebApplication app) =>
        app.UseProductEndpoints();

    private static WebApplication UseProductEndpoints(this WebApplication app)
    {
        app.MapGet("/search/products", SearchEndpoint.Products).Produces((int)HttpStatusCode.OK, typeof(IReadOnlyCollection<Product>));

        return app;
    }
}