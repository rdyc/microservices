using System.Text.Json.Serialization;

namespace Lookup.Attributes;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AttributeType
{
    Text,
    Number,
    Decimal
}