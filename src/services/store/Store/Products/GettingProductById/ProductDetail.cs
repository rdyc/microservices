using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using Store.Attributes;
using Store.Currencies;

namespace Store.Products.GettingProductById;

[BsonCollection("product_detail")]
public record ProductDetail : Document
{
    [BsonElement("SKU")]
    public string SKU { get; set; } = default!;

    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("desc")]
    public string Description { get; set; } = default!;

    [BsonElement("status")]
    public ProductStatus Status { get; set; } = default!;

    [BsonElement("currency")]
    public Currency Currency { get; set; } = default!;

    [BsonElement("price")]
    public decimal Price { get; set; } = default!;

    [BsonElement("stock")]
    public int Stock { get; set; } = default!;

    [BsonElement("attributes")]
    public IList<ProductDetailAttribute> Attributes { get; private set; } = default!;

    [BsonElement("version")]
    public int Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
}

public record ProductDetailAttribute
{
    [BsonElement("id")]
    public Guid Id { get; set; } = default!;

    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("type")]
    public AttributeType Type { get; set; } = default!;

    [BsonElement("unit")]
    public string Unit { get; set; } = default!;

    [BsonElement("value")]
    public string Value { get; set; } = default!;
}