using System.Net;
using FW.Core.Pagination;
using Lookup.Attributes.GettingAttributes;
using Lookup.Currencies.GettingCurrencies;
using Lookup.Histories.GettingHistories;

namespace Lookup.WebApi.Endpoints;

internal static class EndpointBuilder
{
    public static IApplicationBuilder UseLookupEndpoints(this WebApplication app) =>
        app
            .UseCurrencyEndpoints()
            .UseAttributeEndpoints()
            .UseHistoryEndpoints();

    private static WebApplication UseCurrencyEndpoints(this WebApplication app)
    {
        app.MapGet("/currencies", CurrencyEndpoint.Currencies).Produces((int)HttpStatusCode.OK, typeof(IListPaged<CurrencyShortInfo>));
        app.MapGet("/currencies/{id}", CurrencyEndpoint.Currency).Produces((int)HttpStatusCode.OK, typeof(CurrencyShortInfo));
        app.MapGet("/currencies/list", CurrencyEndpoint.CurrencyList).Produces((int)HttpStatusCode.OK, typeof(IListUnpaged<CurrencyShortInfo>));
        app.MapPost("/currencies", CurrencyEndpoint.Create).Produces((int)HttpStatusCode.Created, typeof(Guid));
        app.MapPut("/currencies/{id}", CurrencyEndpoint.Update).Produces((int)HttpStatusCode.Accepted, typeof(Guid));
        app.MapDelete("/currencies/{id}", CurrencyEndpoint.Delete).Produces((int)HttpStatusCode.NoContent);

        return app;
    }

    private static WebApplication UseAttributeEndpoints(this WebApplication app)
    {
        app.MapGet("/attributes", AttributeEndpoint.Attributes).Produces((int)HttpStatusCode.OK, typeof(IListPaged<AttributeShortInfo>));
        app.MapGet("/attributes/{id}", AttributeEndpoint.Attribute).Produces((int)HttpStatusCode.OK, typeof(AttributeShortInfo));
        app.MapGet("/attributes/list", AttributeEndpoint.AttributeList).Produces((int)HttpStatusCode.OK, typeof(IListUnpaged<AttributeShortInfo>));
        app.MapPost("/attributes", AttributeEndpoint.Create).Produces((int)HttpStatusCode.Created, typeof(Guid));
        app.MapPut("/attributes/{id}", AttributeEndpoint.Update).Produces((int)HttpStatusCode.Accepted, typeof(Guid));
        app.MapDelete("/attributes/{id}", AttributeEndpoint.Delete).Produces((int)HttpStatusCode.NoContent);

        return app;
    }

    private static WebApplication UseHistoryEndpoints(this WebApplication app)
    {
        app.MapGet("/histories/{id}", HistoryEndpoint.Histories).Produces((int)HttpStatusCode.OK, typeof(IListPaged<History>));

        return app;
    }
}