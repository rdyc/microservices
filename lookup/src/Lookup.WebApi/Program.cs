using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventStore.Client;
using FW.Core;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.Exceptions;
using FW.Core.Kafka;
using FW.Core.WebApi.Middlewares.ExceptionHandling;
using FW.Core.WebApi.OptimisticConcurrency;
using FW.Core.WebApi.Tracing;
using Lookup;
using Lookup.WebApi.Endpoints;
using Microsoft.AspNetCore.Http.Json;
using Swashbuckle.AspNetCore.SwaggerUI;

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
    .AddCoreServices()
    .AddKafkaProducer()
    .AddCorrelationIdMiddleware()
    .AddOptimisticConcurrencyMiddleware(
        sp => sp.GetRequiredService<EventStoreDBExpectedStreamRevisionProvider>().TrySet,
        sp => () => sp.GetRequiredService<EventStoreDBNextStreamRevisionProvider>().Value?.ToString()
    )
    .Configure<FW.Core.MongoDB.Settings.MongoDbSettings>(builder.Configuration.GetSection(nameof(FW.Core.MongoDB.Settings.MongoDbSettings)))
    .AddLookup(config);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles()
        .UseSwagger()
        .UseSwaggerUI(options =>
        {
            options.DocExpansion(DocExpansion.None);
            options.EnableTryItOutByDefault();
            options.DisplayOperationId();
            options.DisplayRequestDuration();
            options.InjectStylesheet("css/theme-dark.css");
            options.InjectJavascript("js/json-folding.js");
        });
}

app.UseExceptionHandlingMiddleware(exception => exception switch
    {
        AggregateNotFoundException _ => HttpStatusCode.NotFound,
        WrongExpectedVersionException => HttpStatusCode.PreconditionFailed,
        _ => HttpStatusCode.InternalServerError
    })
    .UseCorrelationIdMiddleware()
    .UseOptimisticConcurrencyMiddleware();

app.UseLookupEndpoints();

app.Run();