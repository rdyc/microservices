using System.Text.Json;
using System.Text.Json.Serialization;
using FW.Core;
using FW.Core.Consul;
using FW.Core.Kafka;
using FW.Core.WebApi.Middlewares;
using FW.Core.WebApi.Tracing;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;
using Search;
using Search.WebApi.Endpoints;
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
        options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        var openApiInfo = new OpenApiInfo
        {
            Version = "v1",
            Title = "Search API",
            Description = "An unambitious e-commerce for search service",
            Contact = new OpenApiContact
            {
                Name = "Ruddy Cahyadi",
                Email = "ruddycahyadi@gmail.com",
                Url = new Uri("https://github.com/rdyc")
            },
            License = new OpenApiLicense
            {
                Name = "GNU General Public License",
                Url = new Uri("https://raw.githubusercontent.com/rdyc/microservices/main/LICENSE")
            }
        };

        options.SwaggerDoc("v1", openApiInfo);
        options.EnableAnnotations();
        options.DescribeAllParametersInCamelCase();
    })
    .AddCoreServices()
    .AddKafkaConsumer()
    .AddCorrelationIdMiddleware()
    .AddSearchServices(config)
    .AddConsul(config)
    .AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles()
        .UseSwagger()
        .UseSwaggerUI(options =>
        {
            options.DocumentTitle = "Search";
            options.DocExpansion(DocExpansion.None);
            options.EnableTryItOutByDefault();
            options.DisplayOperationId();
            options.DisplayRequestDuration();
            options.InjectStylesheet("css/theme-dark.css");
            options.InjectJavascript("js/json-folding.js");
        })
        .UseSwaggerUIThemes()
        .UseSwaggerUIPlugins();
}

app.UseResponseTimeMiddleware()
    .UseCorrelationIdMiddleware()
    .UseExceptionHandlingMiddleware();

app.MapHealthChecks("/hc");

app.UseSearchEndpoints()
    .UseConsul(app.Lifetime);

app.Run();