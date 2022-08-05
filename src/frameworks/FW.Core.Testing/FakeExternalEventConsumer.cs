using FW.Core.Events.External;

namespace FW.Core.Testing;

public class FakeExternalEventConsumer : IExternalEventConsumer
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}