using System.Text.Json.Serialization;

namespace Lookup.Currencies;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CurrencyStatus
{
    Active,
    Inactive,
    Removed
}