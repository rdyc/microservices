using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Store.Currencies;

[BsonCollection("currency")]
public record Currency : Document
{
    [BsonElement("name")]
    public string Name { get; private set; } = default!;
    
    [BsonElement("code")]
    public string Code { get; private set; } = default!;
    
    [BsonElement("symbol")]
    public string Symbol { get; private set; } = default!;
}