using FW.Core.Events;
using FW.Core.MongoDB;
using Lookup.Currencies.ModifyingCurrency;
using Lookup.Currencies.RegisteringCurrency;
using Lookup.Currencies.RemovingCurrency;
using MongoDB.Bson.Serialization.Attributes;

namespace Lookup.Currencies.GettingCurrencies;

[BsonCollection("currency_shortinfo")]
public record CurrencyShortInfo : Document
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

public class CurrencyShortInfoProjection
{
    public static CurrencyShortInfo Handle(EventEnvelope<CurrencyRegistered> eventEnvelope)
    {
        var (id, name, code, symbol, status) = eventEnvelope.Data;

        return new CurrencyShortInfo
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

    public static void Handle(EventEnvelope<CurrencyModified> eventEnvelope, CurrencyShortInfo view)
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

    public static void Handle(EventEnvelope<CurrencyRemoved> eventEnvelope, CurrencyShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = LookupStatus.Removed;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }
}