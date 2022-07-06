using System.Text.Json.Serialization;

namespace Lookup;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LookupStatus
{
    Active,
    Inactive,
    Removed
}