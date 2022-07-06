using FW.Core.Events;
using Lookup.Currencies.ModifyingCurrency;
using Lookup.Currencies.RegisteringCurrency;
using Lookup.Currencies.RemovingCurrency;
using Lookup.Histories.GettingHistories;

namespace Lookup.Currencies;

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