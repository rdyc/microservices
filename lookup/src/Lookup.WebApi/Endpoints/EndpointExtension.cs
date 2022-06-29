using System.Net;
using FW.Core.Pagination;
using Lookup.Currencies.GettingCurrencies;

namespace Lookup.WebApi.Endpoints;

internal static class EndpointExtension
{
    public static IApplicationBuilder UseLookupEndpoints(this WebApplication app) =>
        app
            .UseCurrencyEndpoints()
            .UseAttributeEndpoints();

    private static WebApplication UseCurrencyEndpoints(this WebApplication app)
    {
        app.MapGet("/currencies", CurrencyEndpoint.GetAsync).Produces((int)HttpStatusCode.OK, typeof(IListPaged<CurrencyShortInfo>)).WithName("get_currencies");
        app.MapPost("/currencies", CurrencyEndpoint.PostAsync).Produces((int)HttpStatusCode.OK, typeof(Guid));
        app.MapPut("/currencies/{id}", CurrencyEndpoint.PutAsync).Produces((int)HttpStatusCode.Accepted, typeof(Guid));

        return app;
    }

    private static WebApplication UseAttributeEndpoints(this WebApplication app)
    {
        app.MapGet("/attributes", AttributeEndpoint.GetAsync).Produces((int)HttpStatusCode.OK, typeof(IEnumerable<CurrencyShortInfo>)).WithName("get_attributes");
        app.MapPost("/attributes", AttributeEndpoint.PostAsync).Produces((int)HttpStatusCode.OK, typeof(Guid));
        app.MapPut("/attributes/{id}", AttributeEndpoint.PutAsync).Produces((int)HttpStatusCode.Accepted, typeof(Guid));

        return app;
    }
}