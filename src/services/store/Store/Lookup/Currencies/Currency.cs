using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

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
}