using System;
using System.Text.Json.Serialization;

namespace Product.WebApi.Versions.V1.Models;

/// <summary>
/// The update currency request.
/// </summary>
public class UpdateCurrencyRequest : CurrencyRequest
{
    /// <summary>
    /// The currency id.
    /// </summary>
    /// <value>The currency id.</value>
    [JsonIgnore]
    public Guid Id { get; set; }
}