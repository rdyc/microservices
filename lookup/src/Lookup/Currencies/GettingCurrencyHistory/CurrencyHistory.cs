using FW.Core.Events;
using FW.Core.MongoDB;
using Lookup.Currencies.Modifying;
using Lookup.Currencies.Registering;
using Lookup.Currencies.Removing;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Lookup.Currencies.GettingCurrencyHistory;

[BsonCollection("currency_history")]
public record CurrencyHistory : Document
{
    [BsonElement("event_id")]
    public string EventId { get; set; } = default!;

    [BsonElement("aggregate_id")]
    public Guid AggregateId { get; set; } = default!;

    [BsonElement("type")]
    public string Type { get; set; } = default!;

    [BsonElement("data")]
    public IDictionary<string, object>? Data { get; set; }
}

public class CurrencyHistoryProjection
{
    public static CurrencyHistory Handle(EventEnvelope<CurrencyRegistered> eventEnvelope)
    {
        var (id, name, code, symbol, status) = eventEnvelope.Data;

        var data = new Dictionary<string, object>
        {
            { "name", name },
            { "code", code },
            { "symbol", symbol },
            { "status", status }
        };

        return new CurrencyHistory
        {
            Id = Guid.NewGuid(),
            EventId = eventEnvelope.Metadata.EventId,
            AggregateId = id,
            Type = nameof(CurrencyRegistered),
            Data = data
        };
    }

    public static CurrencyHistory Handle(EventEnvelope<CurrencyModified> eventEnvelope)
    {
        var (id, name, code, symbol) = eventEnvelope.Data;

        var data = new Dictionary<string, object>
        {
            { "name", name },
            { "code", code },
            { "symbol", symbol }
        };

        return new CurrencyHistory
        {
            Id = Guid.NewGuid(),
            EventId = eventEnvelope.Metadata.EventId,
            AggregateId = id,
            Type = nameof(CurrencyModified),
            Data = data
        };
    }

    public static CurrencyHistory Handle(EventEnvelope<CurrencyRemoved> eventEnvelope)
    {
        return new CurrencyHistory
        {
            Id = Guid.NewGuid(),
            EventId = eventEnvelope.Metadata.EventId,
            AggregateId = eventEnvelope.Data.Id,
            Type = nameof(CurrencyRemoved)
        };
    }
}