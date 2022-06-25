using FW.Core.Events;
using FW.Core.EventStoreDB.Serialization;
using EventStore.Client;

namespace FW.Core.EventStoreDB.Events;

public static class EventEnvelopeExtensions
{
    public static IEventEnvelope? ToEventEnvelope(this ResolvedEvent resolvedEvent)
    {
        var eventData = resolvedEvent.Deserialize();
        var eventMetadata = resolvedEvent.DeserializeMetadata();

        if (eventData == null)
            return null;

        var metaData = new EventMetadata(
            resolvedEvent.Event.EventId.ToString(),
            resolvedEvent.Event.EventNumber.ToUInt64(),
            resolvedEvent.Event.Position.CommitPosition,
            eventMetadata
        );

        return EventEnvelopeFactory.From(eventData, metaData);
    }
}
