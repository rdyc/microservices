namespace Product.WebApi.Versions.V1.Models;

/// <summary>
/// The currency request.
/// </summary>
public class CurrencyRequest
{
    /// <summary>
    /// The currency name.
    /// </summary>
    /// <value>The currency name.</value>
    public string Name { get; set; }
    
    /// <summary>
    /// The currency code.
    /// </summary>
    /// <value>The currency code.</value>
    public string Code { get; set; }

    /// <summary>
    /// The currency symbol.
    /// </summary>
    /// <value>The currency symbol.</value>
    public string Symbol { get; set; }
}