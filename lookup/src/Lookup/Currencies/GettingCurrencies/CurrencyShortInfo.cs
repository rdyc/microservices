using FW.Core.Events;
using FW.Core.MongoDB;
using Lookup.Currencies.Modifying;
using Lookup.Currencies.Registering;
using Lookup.Currencies.Removing;
using MongoDB.Bson.Serialization.Attributes;

namespace Lookup.Currencies.GettingCurrencies;

[BsonCollection("currency_shortinfo")]
public record CurrencyShortInfo : Document//, IVersionedProjection
{
    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("code")]
    public string Code { get; set; } = default!;

    [BsonElement("symbol")]
    public string Symbol { get; set; } = default!;

    [BsonElement("status")]
    public CurrencyStatus Status { get; set; } = default!;

    [BsonElement("version")]
    public int Version { get; set; }

    [BsonElement("position")]
    public ulong LastProcessedPosition { get; set; }
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
            Version = 0,
            LastProcessedPosition = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<CurrencyModified> eventEnvelope, CurrencyShortInfo view)
    {
        if (view.LastProcessedPosition >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, name, code, symbol) = eventEnvelope.Data;

        view.Name = name;
        view.Code = code;
        view.Symbol = symbol;
        view.Version++;
        view.LastProcessedPosition = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<CurrencyRemoved> eventEnvelope, CurrencyShortInfo view)
    {
        if (view.LastProcessedPosition >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = CurrencyStatus.Removed;
        view.Version++;
        view.LastProcessedPosition = eventEnvelope.Metadata.LogPosition;
    }
}