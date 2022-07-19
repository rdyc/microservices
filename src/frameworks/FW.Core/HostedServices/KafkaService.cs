using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FW.Core.BackgroundServices;

public class KafkaService : BackgroundService
{
    private readonly ILogger<KafkaService> logger;
    private readonly Func<CancellationToken, Task> perform;

    public KafkaService(
        ILogger<KafkaService> logger,
        Func<CancellationToken, Task> perform
    )
    {
        this.logger = logger;
        this.perform = perform;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => 
        Task.Run(async () =>
        {
            await Task.Yield();
            logger.LogInformation("Kafka service stopped");
            await perform(stoppingToken);
            logger.LogInformation("Kafka service stopped");
        }, stoppingToken);
}