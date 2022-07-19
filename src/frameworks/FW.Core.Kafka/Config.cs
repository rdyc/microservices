using FW.Core.BackgroundServices;
using FW.Core.Events;
using FW.Core.Events.External;
using FW.Core.Kafka.Consumers;
using FW.Core.Kafka.Producers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace FW.Core.Kafka;

public static class Config
{
    public static IServiceCollection AddKafkaProducer(this IServiceCollection services)
    {
        //using TryAdd to support mocking, without that it won't be possible to override in tests
        services.TryAddSingleton<IExternalEventProducer, KafkaProducer>();
        services.AddSingleton<IEventBus>(sp =>
            new EventBusDecoratorWithExternalProducer(sp.GetRequiredService<EventBus>(),
                sp.GetRequiredService<IExternalEventProducer>()));
        return services;
    }

    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services)
    {
        //using TryAdd to support mocking, without that it won't be possible to override in tests
        services.TryAddSingleton<IExternalEventConsumer, KafkaConsumer>();

        return services.AddHostedService(serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<KafkaService>>();
                var consumer = serviceProvider.GetRequiredService<IExternalEventConsumer>();

                return new KafkaService(logger, consumer.StartAsync);
            }
        );
    }

    public static IServiceCollection AddKafkaProducerAndConsumer(this IServiceCollection services)
    {
        return services
            .AddKafkaProducer()
            .AddKafkaConsumer();
    }
}
