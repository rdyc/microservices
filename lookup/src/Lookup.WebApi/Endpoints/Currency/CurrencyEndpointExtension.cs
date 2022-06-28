using System.Net;
using Lookup.Currencies.GettingCurrencies;

namespace Lookup.WebApi.Endpoints
{
    public static class CurrencyEndpointExtension
    {
        public static void UseCurrencyEndpoint(this WebApplication app)
        {
            app.MapGet("/currencies", CurrencyEndpoint.GetAsync).Produces((int)HttpStatusCode.OK, typeof(IEnumerable<CurrencyShortInfo>)).WithName("get_all_currencies");
            app.MapPost("/currencies", CurrencyEndpoint.PostAsync).Produces((int)HttpStatusCode.OK, typeof(Guid));
            app.MapPut("/currencies/{id}", CurrencyEndpoint.PutAsync).Produces((int)HttpStatusCode.Accepted, typeof(Guid));
        }
    }
}