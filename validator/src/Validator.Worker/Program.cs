using FW.Core;
using FW.Core.Kafka;
using FW.Core.MongoDB.Settings;
using Validator;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services
            .Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)))
            // .AddKafkaConsumer()
            .AddCoreServices()
            .AddValidator(configuration);

        // services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();