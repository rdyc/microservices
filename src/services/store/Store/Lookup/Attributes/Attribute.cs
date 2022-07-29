using System.Text.Json.Serialization;
using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using Store.Lookup.Attributes.ModifyingAttribute;
using Store.Lookup.Attributes.RegisteringAttribute;
using Store.Lookup.Attributes.RemovingAttribute;

namespace Store.Lookup.Attributes;

[BsonCollection("attribute")]
public record Attribute : Document
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

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AttributeType
{
    Text,
    Number,
    Decimal
}

public class AttributeProjection
{
    public static Attribute Handle(EventEnvelope<AttributeRegistered> eventEnvelope)
    {
        var (id, name, type, unit, status) = eventEnvelope.Data;

        return new Attribute
        {
            Id = id,
            Name = name,
            Type = type,
            Unit = unit,
            Status = status,
            Version = eventEnvelope.Metadata.StreamPosition,
            Position = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<AttributeModified> eventEnvelope, Attribute view)
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

    public static void Handle(EventEnvelope<AttributeRemoved> eventEnvelope, Attribute view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = LookupStatus.Removed;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }
}