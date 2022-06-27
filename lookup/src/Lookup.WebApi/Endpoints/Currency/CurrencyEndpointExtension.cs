using System.Net;

namespace Lookup.WebApi.Endpoints
{
    public static class CurrencyEndpointExtension
    {
        public static void UseCurrencyEndpoint(this WebApplication app)
        {
            app.MapPost("/currencies", CurrencyEndpoint.PostAsync).Produces((int)HttpStatusCode.OK, typeof(Guid));
        }
    }
}