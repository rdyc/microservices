using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Store.Attributes;

[BsonCollection("attribute")]
public record Attribute : Document
{
    [BsonElement("name")]
    public string Name { get; } = default!;

    [BsonElement("type")]
    public AttributeType Type { get; } = default!;

    [BsonElement("unit")]
    public string Unit { get; } = default!;
}