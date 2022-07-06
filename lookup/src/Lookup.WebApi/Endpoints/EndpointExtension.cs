using System.Net;
using FW.Core.Pagination;
using Lookup.Currencies.GettingCurrencies;
using Lookup.Currencies.GettingCurrencyHistory;

namespace Lookup.WebApi.Endpoints;

internal static class EndpointExtension
{
    public static IApplicationBuilder UseLookupEndpoints(this WebApplication app) =>
        app
            .UseCurrencyEndpoints()
            .UseAttributeEndpoints();

    private static WebApplication UseCurrencyEndpoints(this WebApplication app)
    {
        app.MapGet("/currencies", CurrencyEndpoint.GetAllAsync).Produces((int)HttpStatusCode.OK, typeof(IListPaged<CurrencyShortInfo>));
        app.MapGet("/currencies/{id}", CurrencyEndpoint.GetDetailAsync).Produces((int)HttpStatusCode.OK, typeof(CurrencyShortInfo)).WithName("get_currency");
        app.MapGet("/currencies/list", CurrencyEndpoint.GetListAsync).Produces((int)HttpStatusCode.OK, typeof(IListUnpaged<CurrencyShortInfo>));
        app.MapGet("/currencies/{id}/histories", CurrencyEndpoint.GetHistoryAsync).Produces((int)HttpStatusCode.OK, typeof(IListPaged<CurrencyHistory>));
        app.MapPost("/currencies", CurrencyEndpoint.PostAsync).Produces((int)HttpStatusCode.OK, typeof(Guid));
        app.MapPut("/currencies/{id}", CurrencyEndpoint.PutAsync).Produces((int)HttpStatusCode.Accepted, typeof(Guid));
        app.MapDelete("/currencies/{id}", CurrencyEndpoint.DeleteAsync).Produces((int)HttpStatusCode.NoContent);

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