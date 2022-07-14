using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FW.Core.HostedServices;

public class BackgroundHostedService: IHostedService
{
    private readonly ILogger<BackgroundHostedService> logger;
    private readonly Func<CancellationToken, Task> perform;

    public BackgroundHostedService(
        ILogger<BackgroundHostedService> logger,
        Func<CancellationToken, Task> perform
    )
    {
        this.logger = logger;
        this.perform = perform;
    }

    public Task StartAsync(CancellationToken cancellationToken)
        => Task.Run(async () =>
        {
            await Task.Yield();
            logger.LogInformation("Background hosted service stopped");
            await perform(cancellationToken);
            logger.LogInformation("Background hosted service stopped");
        }, cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Background hosted service is stopping.");

        return Task.CompletedTask;
    }
}