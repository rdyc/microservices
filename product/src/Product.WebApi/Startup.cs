using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Product.Domain;
using Product.WebApi.Configurations.Swagger;
using Product.WebApi.Configurations.Swagger.DocumentFilter;
using Product.WebApi.Configurations.Swagger.OperationFilter;
using Product.WebApi.Versions.V1;
using Product.WebApi.Versions.V2;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Product.WebApi;

internal class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                    .WithExposedHeaders("Date", "X-Response-Time-Ms", "X-Correlation-Id", "X-Rate-Limit-Limit", "X-Rate-Limit-Remaining", "X-Rate-Limit-Reset"));
            })
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = false;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
            });

        services
            .AddMediatR(typeof(Startup))
            .AddV1Service()
            .AddV2Service()
            .AddDomainContext(options =>
            {
                var config = new
                {
                    Host = "xdev.equine.co.id",
                    Port = 5432,
                    Name = "x.product",
                    Version = string.Empty,
                    UserId = "app_user",
                    Password = "P@ssw0rd!"
                };

                options.UseNpgsql($"Server={config.Host};Port={config.Port};Database={config.Name};User Id={config.UserId};Password={config.Password}", action =>
                {
                    if (!string.IsNullOrEmpty(config.Version))
                    {
                        action.SetPostgresVersion(Version.Parse(config.Version));
                    }
                });

                options
                    .EnableSensitiveDataLogging(true)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

        services.AddApiVersioning(o => o.ReportApiVersions = true)
            .AddMvcCore()
            .AddApiExplorer();

        services
            .AddVersionedApiExplorer(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.GroupNameFormat = "'v'VVV";
            });

        services.AddSwaggerGen(config =>
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(AppContext.BaseDirectory));
            foreach (var fi in dir.EnumerateFiles("*.xml"))
            {
                config.IncludeXmlComments(fi.FullName);
            }

            config.EnableAnnotations();
            config.DescribeAllParametersInCamelCase();

            // document filters
            config.DocumentFilter<ReplaceVersionWithExactValueInPath>();

            // operation filters
            config.OperationFilter<RemoveVersionParameterOperationFilter>();
        });

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddDomainService();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, IMapper mapper)
    {
        mapper.ConfigurationProvider.AssertConfigurationIsValid();

        // app.UseDbMigration();

        app.UseCors("CorsPolicy");

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage()
                .UseSwagger()
                .UseSwaggerUI(config =>
                {
                    foreach (var version in provider.ApiVersionDescriptions)
                    {
                        var endpoint = $"/swagger/{version.GroupName}/swagger.json";

                        config.SwaggerEndpoint(endpoint, $"Product API {version.GroupName.ToUpperInvariant()}");
                    }

                    config.DocumentTitle = $"Product API ({env.EnvironmentName})";
                    config.DocExpansion(DocExpansion.None);
                    config.DefaultModelRendering(ModelRendering.Model);
                    config.DisplayRequestDuration();
                    config.EnableDeepLinking();
                    config.EnableFilter();
                });
        }

        app.UseHttpsRedirection()
            .UseRouting()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
    }
}