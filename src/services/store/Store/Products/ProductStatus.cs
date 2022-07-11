using System.Text.Json.Serialization;

namespace Store.Products;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProductStatus
{
    Available,
    Discontinue
}