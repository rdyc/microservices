using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

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
}