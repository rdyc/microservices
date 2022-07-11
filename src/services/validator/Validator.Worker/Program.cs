using FW.Core;
using FW.Core.Kafka;
using Validator;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        services
            .AddKafkaConsumer()
            .AddCoreServices()
            .AddValidatorServices(configuration);
    })
    .Build();

await host.RunAsync();