using System.Text.Json.Serialization;

namespace Cart.Products;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProductStatus
{
    Available,
    Discontinue
}