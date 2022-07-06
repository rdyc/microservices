using FW.Core.MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Lookup.Histories.GettingHistories;

[BsonCollection("history")]
public record History : Document
{
    [BsonElement("aggregate_id")]
    public Guid AggregateId { get; set; } = default!;

    [BsonElement("type")]
    public string Type { get; set; } = default!;

    [BsonElement("data")]
    public IDictionary<string, object>? Data { get; set; }
}