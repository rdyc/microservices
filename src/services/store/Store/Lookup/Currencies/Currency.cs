using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using Store.Lookup.Currencies.ModifyingCurrency;
using Store.Lookup.Currencies.RegisteringCurrency;
using Store.Lookup.Currencies.RemovingCurrency;

namespace Store.Lookup.Currencies;

[BsonCollection("currency")]
public record Currency : Document
{
    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("code")]
    public string Code { get; set; } = default!;

    [BsonElement("symbol")]
    public string Symbol { get; set; } = default!;

    [BsonElement("status")]
    public LookupStatus Status { get; set; } = default!;

    [BsonElement("version")]
    public ulong Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
}

public class CurrencyProjection
{
    public static Currency Handle(EventEnvelope<CurrencyRegistered> eventEnvelope)
    {
        var (id, name, code, symbol, status) = eventEnvelope.Data;

        return new Currency
        {
            Id = id,
            Name = name,
            Code = code,
            Symbol = symbol,
            Status = status,
            Version = eventEnvelope.Metadata.StreamPosition,
            Position = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<CurrencyModified> eventEnvelope, Currency view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, name, code, symbol) = eventEnvelope.Data;

        view.Name = name;
        view.Code = code;
        view.Symbol = symbol;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<CurrencyRemoved> eventEnvelope, Currency view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = LookupStatus.Removed;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }
}