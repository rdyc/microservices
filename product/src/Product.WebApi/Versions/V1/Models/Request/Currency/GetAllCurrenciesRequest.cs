using Product.Contract.Queries;

namespace Product.WebApi.Versions.V1.Models;

/// <summary>
/// The get all currencies request.
/// </summary>
public class GetAllCurrenciesRequest
{
    /// <summary>
    /// The find criteria.
    /// </summary>
    /// <value>The find criteria.</value>
    public string Find { get; set; }

    /// <summary>
    /// The find by field criteria.
    /// </summary>
    /// <value>The find by field criteria.</value>
    public CurrencyField? FindBy { get; set; }

    /// <summary>
    /// The page index.
    /// </summary>
    /// <value>The page index.</value>
    public int? Index { get; set; }

    /// <summary>
    /// The page size.
    /// </summary>
    /// <value>The page size.</value>
    public int? Size { get; set; }
}