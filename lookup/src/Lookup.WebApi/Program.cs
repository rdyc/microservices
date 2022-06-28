using System.Text.Json;
using System.Text.Json.Serialization;
using FW.Core;
using Lookup;
using Lookup.WebApi.Endpoints;
using Lookup.WebApi.Repositories;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

using var loggerFactory = LoggerFactory.Create(config =>
    config.SetMinimumLevel(LogLevel.Trace)
        .AddConsole()
);

var config = builder.Configuration;

// Add services to the container.
builder.Services
    .Configure<JsonOptions>(options =>
    {
        options.SerializerOptions.WriteIndented = false;
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
    })
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.EnableAnnotations();
        options.DescribeAllParametersInCamelCase();
    })
    .AddSingleton<IWeatherRepository, WeatherRepository>()
    .AddCoreServices()
    .Configure<FW.Core.MongoDB.Settings.MongoDbSettings>(builder.Configuration.GetSection(nameof(FW.Core.MongoDB.Settings.MongoDbSettings)))
    .AddLookup(config);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();
app.UseWeatherEndpoint();
app.UseCurrencyEndpoint();

app.Run();