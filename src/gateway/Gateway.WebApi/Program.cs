using FW.Core.WebApi.Middlewares;
using Ocelot.Administration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddJsonFile("ocelot.json", false, true)
    .AddEnvironmentVariables();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddSwaggerForOcelot(builder.Configuration)
    .AddOcelot()
    .AddConsul()
    .AddAdministration("/admin", "secret");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerForOcelotUI(options =>
    {
        options.PathToSwaggerGenerator = "/swagger/docs";
        options.DocumentTitle = "API Gateway";
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

app.UseOcelot().Wait();

app.Run();