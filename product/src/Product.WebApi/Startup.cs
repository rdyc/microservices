using System;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Product.Domain;
using Product.Domain.MapProfile;
using Product.WebApi.Converters;

namespace Product.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                    .WithExposedHeaders("Date", "X-Response-Time-Ms", "X-Correlation-Id", "X-Rate-Limit-Limit", "X-Rate-Limit-Remaining", "X-Rate-Limit-Reset"));
            });

            services.AddControllers();

            services.AddAutoMapper(config =>
            {
                config.AddProfile<RequestToCommandProfile>();
                config.AddProfile<RequestToQueryProfile>();
                config.AddProfile<EntityToDtoProfile>();
            });

            services.AddScoped<CriteriaConverter>();
            services.AddScoped<PagedConverter>();
            services.AddScoped<OrderedConverter>();

            services.AddMediatR(typeof(Startup));

            // services.AddValidatorsFromAssembly(System.Reflection.Assembly.Load("Product.Domain"));

            services.AddDomainContext(options =>
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
                    .EnableSensitiveDataLogging(false)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddDomainService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            app.UseDbMigration();

            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public static class DbMiddleware
    {
        public static void UseDbMigration(this IApplicationBuilder app)
        {
            try
            {
                using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
                ServiceExtension.UseDbMigration(scope);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.GetBaseException().Message);
            }
        }
    }
}