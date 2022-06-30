using Confluent.Kafka;
using FW.Core.Events;
using FW.Core.Reflection;
using FW.Core.Serialization.Newtonsoft;

namespace FW.Core.Kafka.Events;

public static class EventEnvelopeExtensions
{
    public static IEventEnvelope? ToEventEnvelope(this ConsumeResult<string, string> message)
    {
        var eventType = TypeProvider.GetTypeFromAnyReferencingAssembly(message.Message.Key);

        if (eventType == null)
            return null;

        var eventEnvelopeType = typeof(EventEnvelope<>).MakeGenericType(eventType);

        // deserialize event
        return message.Message.Value.FromJson(eventEnvelopeType) as IEventEnvelope;
    }
}
