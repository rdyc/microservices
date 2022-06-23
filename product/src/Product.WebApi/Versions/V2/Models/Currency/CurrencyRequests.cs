namespace Product.WebApi.Versions.V2.Models;

/// <summary>
/// Create currency request.
/// </summary>
/// <param name="Name">The currency name.</param>
/// <param name="Code">The currency code.</param>
/// <param name="Symbol">The currency symbol.</param>
public record CreateCurrencyRequest(string Name, string Code, string Symbol);

/// <summary>
/// Update currency request.
/// </summary>
/// <param name="Name">The currency name.</param>
/// <param name="Code">The currency code.</param>
/// <param name="Symbol">The currency symbol.</param>
public record UpdateCurrencyRequest(string Name, string Code, string Symbol);