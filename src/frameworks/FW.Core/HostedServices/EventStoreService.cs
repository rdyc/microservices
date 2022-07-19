using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FW.Core.BackgroundServices;

public class EventStoreService : BackgroundService
{
    private readonly ILogger<EventStoreService> logger;
    private readonly Func<CancellationToken, Task> perform;

    public EventStoreService(
        ILogger<EventStoreService> logger,
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
            logger.LogInformation("EventStore service stopped");
            await perform(stoppingToken);
            logger.LogInformation("EventStore service stopped");
        }, stoppingToken);
}