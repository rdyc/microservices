using FW.Core;
using Validator;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        services
            // .AddKafkaConsumer()
            .AddCoreServices()
            .AddValidator(configuration);
    })
    .Build();

await host.RunAsync();