using System;

namespace Product.WebApi.Versions.V1.Models;

/// <summary>
/// The product request.
/// </summary>
public class ProductRequest
{
    /// <summary>
    /// The product name.
    /// </summary>
    /// <value>The product name.</value>
    public string Name { get; set; }
    
    /// <summary>
    /// The product description.
    /// </summary>
    /// <value>The product description.</value>
    public string Description { get; set; }

    /// <summary>
    /// The product currency id.
    /// </summary>
    /// <value>The product currency id.</value>
    public Guid CurrencyId { get; set; }

    /// <summary>
    /// The product price.
    /// </summary>
    /// <value>The product price.</value>
    public decimal Price { get; set; }
}