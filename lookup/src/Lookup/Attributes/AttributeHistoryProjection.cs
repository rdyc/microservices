using FW.Core.Events;
using Lookup.Attributes.Modifying;
using Lookup.Attributes.Registering;
using Lookup.Attributes.Removing;
using Lookup.Histories.GettingHistories;

namespace Lookup.Attributes;

public class AttributeHistoryProjection
{
    public static History Handle(EventEnvelope<AttributeRegistered> eventEnvelope)
    {
        var (id, name, type, unit, status) = eventEnvelope.Data;

        var data = new Dictionary<string, object>
        {
            { "name", name },
            { "type", type },
            { "unit", unit },
            { "status", status }
        };

        return new History
        {
            Id = Guid.Parse(eventEnvelope.Metadata.EventId),
            AggregateId = id,
            Type = nameof(AttributeRegistered),
            Data = data
        };
    }

    public static History Handle(EventEnvelope<AttributeModified> eventEnvelope)
    {
        var (id, name, type, unit) = eventEnvelope.Data;

        var data = new Dictionary<string, object>
        {
            { "name", name },
            { "type", type },
            { "unit", unit }
        };

        return new History
        {
            Id = Guid.Parse(eventEnvelope.Metadata.EventId),
            AggregateId = id,
            Type = nameof(AttributeModified),
            Data = data
        };
    }

    public static History Handle(EventEnvelope<AttributeRemoved> eventEnvelope)
    {
        return new History
        {
            Id = Guid.Parse(eventEnvelope.Metadata.EventId),
            AggregateId = eventEnvelope.Data.Id,
            Type = nameof(AttributeRemoved)
        };
    }
}