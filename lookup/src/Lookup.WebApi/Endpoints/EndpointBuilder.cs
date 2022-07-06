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
        app.MapGet("/currencies", CurrencyEndpoint.GetAllAsync).Produces((int)HttpStatusCode.OK, typeof(IListPaged<CurrencyShortInfo>));
        app.MapGet("/currencies/{id}", CurrencyEndpoint.GetDetailAsync).Produces((int)HttpStatusCode.OK, typeof(CurrencyShortInfo)).WithName("get_currency");
        app.MapGet("/currencies/list", CurrencyEndpoint.GetListAsync).Produces((int)HttpStatusCode.OK, typeof(IListUnpaged<CurrencyShortInfo>));
        app.MapPost("/currencies", CurrencyEndpoint.PostAsync).Produces((int)HttpStatusCode.Created, typeof(Guid));
        app.MapPut("/currencies/{id}", CurrencyEndpoint.PutAsync).Produces((int)HttpStatusCode.Accepted, typeof(Guid));
        app.MapDelete("/currencies/{id}", CurrencyEndpoint.DeleteAsync).Produces((int)HttpStatusCode.NoContent);

        return app;
    }

    private static WebApplication UseAttributeEndpoints(this WebApplication app)
    {
        app.MapGet("/attributes", AttributeEndpoint.GetAllAsync).Produces((int)HttpStatusCode.OK, typeof(IListPaged<AttributeShortInfo>));
        app.MapGet("/attributes/{id}", AttributeEndpoint.GetDetailAsync).Produces((int)HttpStatusCode.OK, typeof(AttributeShortInfo)).WithName("get_attribute");
        app.MapGet("/attributes/list", AttributeEndpoint.GetListAsync).Produces((int)HttpStatusCode.OK, typeof(IListUnpaged<AttributeShortInfo>));
        app.MapPost("/attributes", AttributeEndpoint.PostAsync).Produces((int)HttpStatusCode.Created, typeof(Guid));
        app.MapPut("/attributes/{id}", AttributeEndpoint.PutAsync).Produces((int)HttpStatusCode.Accepted, typeof(Guid));
        app.MapDelete("/attributes/{id}", AttributeEndpoint.DeleteAsync).Produces((int)HttpStatusCode.NoContent);

        return app;
    }

    private static WebApplication UseHistoryEndpoints(this WebApplication app)
    {
        app.MapGet("/histories/{id}", HistoryEndpoint.GetHistoryAsync).Produces((int)HttpStatusCode.OK, typeof(IListPaged<History>));

        return app;
    }
}