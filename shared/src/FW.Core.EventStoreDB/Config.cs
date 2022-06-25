using FW.Core.BackgroundWorkers;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Subscriptions;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
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
    private const string DefaultConfigKey = "EventStore.ConnectionString";

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
            services
                .AddTransient<ISubscriptionCheckpointRepository, EventStoreDBSubscriptionCheckpointRepository>();
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

        return services.AddHostedService(serviceProvider =>
            {
                var logger =
                    serviceProvider.GetRequiredService<ILogger<BackgroundWorker>>();

                var eventStoreDBSubscriptionToAll =
                    serviceProvider.GetRequiredService<EventStoreDBSubscriptionToAll>();

                return new BackgroundWorker(
                    logger,
                    ct =>
                        eventStoreDBSubscriptionToAll.SubscribeToAll(
                            subscriptionOptions ?? new EventStoreDBSubscriptionToAllOptions(),
                            ct
                        )
                );
            }
        );
    }
}
