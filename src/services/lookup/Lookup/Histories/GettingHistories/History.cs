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

    [BsonElement("description")]
    public string Description { get; set; } = default!;

    [BsonElement("version")]
    public ulong Version { get; set; } = default!;

    [BsonElement("position")]
    public ulong Position { get; set; } = default!;

    public static History Create(Guid aggregateId, string description, EventMetadata metadata)
    {
        var (eventId, streamPosition, logPosition, _) = metadata;

        return new History
        {
            Id = Guid.Parse(eventId),
            AggregateId = aggregateId,
            Description = description,
            Version = streamPosition,
            Position = logPosition
        };
    }
}

public class AttributeHistoryProjection
{
    public static History Handle(EventEnvelope<AttributeRegistered> eventEnvelope)
    {
        var (id, name, type, unit, status) = eventEnvelope.Data;

        return History.Create(
            id,
            $"Registered as name: {name}, type: {type}, unit: {unit} and status: {status}",
            eventEnvelope.Metadata
        );
    }

    public static History Handle(EventEnvelope<AttributeModified> eventEnvelope)
    {
        var (id, name, type, unit) = eventEnvelope.Data;

        return History.Create(
            id,
            $"Modified with name: {name}, type: {type} and unit: {unit}",
            eventEnvelope.Metadata
        );
    }

    public static History Handle(EventEnvelope<AttributeRemoved> eventEnvelope) =>
        History.Create(eventEnvelope.Data.AttributeId, "Removed", eventEnvelope.Metadata);
}

public class CurrencyHistoryProjection
{
    public static History Handle(EventEnvelope<CurrencyRegistered> eventEnvelope)
    {
        var (id, name, code, symbol, status) = eventEnvelope.Data;

        return History.Create(
            id,
            $"Registered as name: {name}, code: {code}, symbol: {symbol} and status: {status}",
            eventEnvelope.Metadata
        );
    }

    public static History Handle(EventEnvelope<CurrencyModified> eventEnvelope)
    {
        var (id, name, code, symbol) = eventEnvelope.Data;

        return History.Create(
            id,
            $"Modified with name: {name}, code: {code} and symbol: {symbol}",
            eventEnvelope.Metadata
        );
    }

    public static History Handle(EventEnvelope<CurrencyRemoved> eventEnvelope) =>
        History.Create(eventEnvelope.Data.CurrencyId, "Removed", eventEnvelope.Metadata);
}