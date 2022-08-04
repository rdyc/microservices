using FW.Core.Events.External;

namespace FW.Core.Testing;

public class DummyExternalEventConsumer: IExternalEventConsumer
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}