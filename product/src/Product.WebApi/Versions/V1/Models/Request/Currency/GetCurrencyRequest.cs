using System;

namespace Product.WebApi.Versions.V1.Models;

/// <summary>
/// The get currency request.
/// </summary>
public class GetCurrencyRequest
{
    /// <summary>
    /// The currency id.
    /// </summary>
    /// <value>The currency id.</value>
    public Guid Id { get; set; }
}