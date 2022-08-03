using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FW.Core.Consul;

public static class ServiceExtensions
{
    public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration) =>
        services
            .Configure<ConsulConfig>(configuration.GetSection("Consul"))
            .AddSingleton<IConsulClient>(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ConsulConfig>>();
                var address = config.Value.Address;

                return new ConsulClient(config => config.Address = new Uri(address));
            });

    public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
    {
        var services = app.ApplicationServices;

        var logger = services.GetRequiredService<ILogger<IConsulClient>>();
        var server = services.GetRequiredService<IServer>();
        var consul = services.GetRequiredService<IConsulClient>();
        var config = services.GetRequiredService<IOptions<ConsulConfig>>();

        var registration = new AgentServiceRegistration
        {
            Name = config.Value.ServiceName,
            Tags = config.Value.Tags
        };

        lifetime.ApplicationStarted.Register(() =>
        {
            var addresses = server.Features.Get<IServerAddressesFeature>();
            var uri = new Uri(addresses.Addresses.First());

            registration.ID = $"{config.Value.ServiceId}-{uri.Host}-{uri.Port}";
            registration.Address = uri.Host;
            registration.Port = uri.Port;

            if (config.Value.EnableCheck)
            {
                registration.Check = new AgentCheckRegistration()
                {
                    Name = $"Http Health Check ({uri}hc)",
                    HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}/hc",
                    Method = "GET",
                    TLSSkipVerify = true,
                    Notes = $"Checked at {DateTime.Now}",
                    Timeout = TimeSpan.FromSeconds(3),
                    Interval = TimeSpan.FromSeconds(10)
                };
            }

            consul.Agent.ServiceDeregister(registration.ID).Wait();
            consul.Agent.ServiceRegister(registration).Wait();

            logger.LogInformation("Registered with consul");
        });

        lifetime.ApplicationStopping.Register(() =>
        {
            consul.Agent.ServiceDeregister(registration.ID).Wait();

            logger.LogInformation("Deregistered from consul");
        });

        return app;
    }
}