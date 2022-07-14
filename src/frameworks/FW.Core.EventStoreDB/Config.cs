using EventStore.Client;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Subscriptions;
using FW.Core.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FW.Core.EventStoreDB;

public class EventStoreDBConfig
{
    public string ConnectionString { get; set; } = default!;
}

public record EventStoreDBOptions(
    bool UseInternalCheckpointing = true
);

public static class EventStoreDBConfigExtensions
{
    // private const string DefaultConfigKey = "EventStore";
    private const string DefaultConfigKey = "EventStore:ConnectionString";

    public static IServiceCollection AddEventStoreDB(this IServiceCollection services, IConfiguration config, EventStoreDBOptions? options = null)
    {
        // var eventStoreDBConfig = config.GetSection(DefaultConfigKey).Get<EventStoreDBConfig>();
        var eventStoreDBConfig = config.GetSection(DefaultConfigKey);

        services
            // .AddSingleton(new EventStoreClient(EventStoreClientSettings.Create(eventStoreDBConfig.ConnectionString)))
            .AddSingleton(new EventStoreClient(EventStoreClientSettings.Create(eventStoreDBConfig.Value)))
            .AddEventStoreDBAppendScope()
            .AddTransient<EventStoreDBSubscriptionToAll, EventStoreDBSubscriptionToAll>();

        if (options?.UseInternalCheckpointing != false)
        {
            services.AddTransient<ISubscriptionCheckpointRepository, EventStoreDBSubscriptionCheckpointRepository>();
        }

        return services;
    }

    public static IServiceCollection AddEventStoreDBSubscriptionToAll(
        this IServiceCollection services,
        EventStoreDBSubscriptionToAllOptions? subscriptionOptions = null,
        bool checkpointToEventStoreDB = true)
    {
        if (checkpointToEventStoreDB)
        {
            services
                .AddTransient<ISubscriptionCheckpointRepository, EventStoreDBSubscriptionCheckpointRepository>();
        }

        return services.AddHostedService(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<BackgroundHostedService>>();
                var esdbSubscription = sp.GetRequiredService<EventStoreDBSubscriptionToAll>();

                return new BackgroundHostedService(logger, cancellationToken =>
                    esdbSubscription.SubscribeToAll(
                        subscriptionOptions ?? new EventStoreDBSubscriptionToAllOptions(),
                        cancellationToken
                    )
                );
            }
        );
    }
}
