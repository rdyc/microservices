using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Store.Products.GettingProducts;

[BsonCollection("product_shortinfo")]
public record ProductShortInfo : Document
{
    [BsonElement("SKU")]
    public string SKU { get; set; } = default!;

    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("desc")]
    public string Description { get; set; } = default!;

    [BsonElement("status")]
    public ProductStatus Status { get; set; } = default!;

    [BsonElement("version")]
    public int Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
}