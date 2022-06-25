using System.Net;
using Lookup.WebApi.Models;

namespace Lookup.WebApi.Endpoints
{
    public static class WeatherEndpointExtension
    {
        public static void UseWeatherEndpoint(this WebApplication app)
        {
            app.MapGet("/forecast", WeatherEndpoint.GetWeathersAsync).Produces((int)HttpStatusCode.OK, typeof(IEnumerable<WeatherForecast>));
        }
    }
}