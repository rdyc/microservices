using FW.Core.Events;
using FW.Core.Events.External;

namespace FW.Core.Testing;

public class FakeExternalEventProducer : IExternalEventProducer
{
    public IList<object> PublishedEvents { get; } = new List<object>();

    public Task Publish(IEventEnvelope @event, CancellationToken ct)
    {
        PublishedEvents.Add(@event.Data);

        return Task.CompletedTask;
    }
}