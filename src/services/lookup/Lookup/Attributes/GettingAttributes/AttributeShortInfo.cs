using FW.Core.Events;
using FW.Core.MongoDB;
using Lookup.Attributes.ModifyingAttribute;
using Lookup.Attributes.RegisteringAttribute;
using Lookup.Attributes.RemovingAttribute;
using MongoDB.Bson.Serialization.Attributes;

namespace Lookup.Attributes.GettingAttributes;

[BsonCollection("attribute")]
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
    public ulong Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
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
            Version = eventEnvelope.Metadata.StreamPosition,
            Position = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<AttributeModified> eventEnvelope, AttributeShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        var (_, name, type, unit) = eventEnvelope.Data;

        view.Name = name;
        view.Type = type;
        view.Unit = unit;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<AttributeRemoved> eventEnvelope, AttributeShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = LookupStatus.Removed;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }
}