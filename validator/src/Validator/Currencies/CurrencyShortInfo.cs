using FW.Core.Events;
using FW.Core.MongoDB;
using Lookup.Currencies.Modifying;
using Lookup.Currencies.Registering;
using Lookup.Currencies.Removing;

namespace Validator.Currencies;

public record CurrencyShortInfo : Document
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Symbol { get; set; } = default!;
    public CurrencyStatus Status { get; set; } = default!;
    public int Version { get; set; }
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