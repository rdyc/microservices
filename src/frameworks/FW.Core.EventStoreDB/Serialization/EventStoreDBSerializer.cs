using System.Text;
using FW.Core.Events;
using FW.Core.EventStoreDB.Events;
using FW.Core.Serialization.Newtonsoft;
using FW.Core.Tracing;
using EventStore.Client;
using Newtonsoft.Json;

namespace FW.Core.EventStoreDB.Serialization;

public static class EventStoreDBSerializer
{
    private static readonly JsonSerializerSettings SerializerSettings;

    static EventStoreDBSerializer()
    {
        SerializerSettings =
            new JsonSerializerSettings().WithNonDefaultConstructorContractResolver();

        SerializerSettings.Converters.Add(new EventStoreDBEventMetadataJsonConverter());
    }

    public static T? Deserialize<T>(this ResolvedEvent resolvedEvent) where T : class =>
        Deserialize(resolvedEvent) as T;

    public static object? Deserialize(this ResolvedEvent resolvedEvent)
    {
        // get type
        var eventType = EventTypeMapper.ToType(resolvedEvent.Event.EventType);

        if (eventType == null)
            return null;

        // deserialize event
        return JsonConvert.DeserializeObject(
            Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span),
            eventType,
            SerializerSettings
        )!;
    }

    public static TraceMetadata? DeserializeMetadata(this ResolvedEvent resolvedEvent)
    {
        // get type
        var eventType = EventTypeMapper.ToType(resolvedEvent.Event.EventType);

        if (eventType == null)
            return null;

        // deserialize event
        return JsonConvert.DeserializeObject<TraceMetadata>(
            Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.Span),
            SerializerSettings
        )!;
    }

    public static EventData ToJsonEventData(this object @event, object? metadata = null) =>
        new(
            Uuid.NewUuid(),
            EventTypeMapper.ToName(@event.GetType()),
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, SerializerSettings)),
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(metadata ?? new { }, SerializerSettings))
        );
}
