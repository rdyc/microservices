using MongoDB.Bson.Serialization.Attributes;

namespace Cart.Products;

public record ProductCurrency
{
    private ProductCurrency(
        Guid id,
        string name,
        string code,
        string symbol
    )
    {
        Id = id;
        Name = name;
        Code = code;
        Symbol = symbol;
    }

    public static ProductCurrency Create(
        Guid id,
        string name,
        string code,
        string symbol
    ) => new(id, name, code, symbol);

    [BsonElement("id")]
    public Guid Id { get; set; } = default!;

    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("code")]
    public string Code { get; set; } = default!;

    [BsonElement("symbol")]
    public string Symbol { get; set; } = default!;
}