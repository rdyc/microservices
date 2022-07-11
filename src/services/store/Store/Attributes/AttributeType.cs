using System.Text.Json.Serialization;

namespace Store.Attributes;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AttributeType
{
    Text,
    Number,
    Decimal
}