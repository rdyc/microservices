using FW.Core.Events;
using FW.Core.MongoDB;
using Lookup.Attributes.ModifyingAttribute;
using Lookup.Attributes.RegisteringAttribute;
using Lookup.Attributes.RemovingAttribute;
using Lookup.Currencies.ModifyingCurrency;
using Lookup.Currencies.RegisteringCurrency;
using Lookup.Currencies.RemovingCurrency;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Lookup.Histories.GettingHistories;

[BsonCollection("history")]
public record History : Document
{
    [BsonElement("aggregate_id")]
    public Guid AggregateId { get; set; } = default!;

    [BsonElement("type")]
    public string Type { get; set; } = default!;

    [BsonElement("data")]
    public IDictionary<string, object>? Data { get; set; }
}

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

public class CurrencyHistoryProjection
{
    public static History Handle(EventEnvelope<CurrencyRegistered> eventEnvelope)
    {
        var (id, name, code, symbol, status) = eventEnvelope.Data;

        var data = new Dictionary<string, object>
        {
            { "name", name },
            { "code", code },
            { "symbol", symbol },
            { "status", status }
        };

        return new History
        {
            Id = Guid.Parse(eventEnvelope.Metadata.EventId),
            AggregateId = id,
            Type = nameof(CurrencyRegistered),
            Data = data
        };
    }

    public static History Handle(EventEnvelope<CurrencyModified> eventEnvelope)
    {
        var (id, name, code, symbol) = eventEnvelope.Data;

        var data = new Dictionary<string, object>
        {
            { "name", name },
            { "code", code },
            { "symbol", symbol }
        };

        return new History
        {
            Id = Guid.Parse(eventEnvelope.Metadata.EventId),
            AggregateId = id,
            Type = nameof(CurrencyModified),
            Data = data
        };
    }

    public static History Handle(EventEnvelope<CurrencyRemoved> eventEnvelope)
    {
        return new History
        {
            Id = Guid.Parse(eventEnvelope.Metadata.EventId),
            AggregateId = eventEnvelope.Data.Id,
            Type = nameof(CurrencyRemoved)
        };
    }
}