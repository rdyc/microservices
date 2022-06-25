using Lookup.WebApi.Models;

namespace Lookup.WebApi.Repositories;

public interface IWeatherRepository
{
    IEnumerable<WeatherForecast> Get();
}

public class WeatherRepository : IWeatherRepository
{
    public IEnumerable<WeatherForecast> Get()
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        return Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateTime.Now.AddDays(index),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ));
    }
}
