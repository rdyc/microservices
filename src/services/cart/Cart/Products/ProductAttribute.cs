using Cart.Lookup.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace Cart.Products;

public record ProductAttribute
{
    private ProductAttribute(Guid id, string name, AttributeType type, string unit, string value)
    {
        Id = id;
        Name = name;
        Type = type;
        Unit = unit;
        Value = value;
    }

    public static ProductAttribute Create(Guid id, string name, AttributeType type, string unit, string value) =>
        new(id, name, type, unit, value);

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