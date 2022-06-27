using FW.Core;
using Lookup;
using Lookup.WebApi.Endpoints;
using Lookup.WebApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

using var loggerFactory = LoggerFactory.Create(config => 
    config.SetMinimumLevel(LogLevel.Trace)
        .AddConsole()
);

var config = builder.Configuration;

// Add services to the container.
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.EnableAnnotations();
        options.DescribeAllParametersInCamelCase();
    })
    .AddSingleton<IWeatherRepository, WeatherRepository>()
    .AddCoreServices()
    .AddLookup(config);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();
app.UseWeatherEndpoint();
app.UseCurrencyEndpoint();

app.Run();