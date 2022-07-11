using FW.Core.Events;
using FW.Core.MongoDB;
using Lookup.Attributes.ModifyingAttribute;
using Lookup.Attributes.RegisteringAttribute;
using Lookup.Attributes.RemovingAttribute;
using MongoDB.Bson.Serialization.Attributes;

namespace Lookup.Attributes.GettingAttributes;

[BsonCollection("attribute_shortinfo")]
public record AttributeShortInfo : Document
{
    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("type")]
    public AttributeType Type { get; set; } = default!;

    [BsonElement("unit")]
    public string Unit { get; set; } = default!;

    [BsonElement("status")]
    public LookupStatus Status { get; set; } = default!;

    [BsonElement("version")]
    public int Version { get; set; }

    [BsonElement("position")]
    public ulong LastProcessedPosition { get; set; }
}

public class AttributeShortInfoProjection
{
    public static AttributeShortInfo Handle(EventEnvelope<AttributeRegistered> eventEnvelope)
    {
        var (id, name, code, symbol, status) = eventEnvelope.Data;

        return new AttributeShortInfo
        {
            Id = id,
            Name = name,
            Type = code,
            Unit = symbol,
            Status = status,
            Version = 0,
            LastProcessedPosition = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<AttributeModified> eventEnvelope, AttributeShortInfo view)
    {
        if (view.LastProcessedPosition >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, name, type, unit) = eventEnvelope.Data;

        view.Name = name;
        view.Type = type;
        view.Unit = unit;
        view.Version++;
        view.LastProcessedPosition = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<AttributeRemoved> eventEnvelope, AttributeShortInfo view)
    {
        if (view.LastProcessedPosition >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = LookupStatus.Removed;
        view.Version++;
        view.LastProcessedPosition = eventEnvelope.Metadata.LogPosition;
    }
}