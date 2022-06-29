using System;

namespace Product.WebApi.Versions.V1.Models;

/// <summary>
/// The delete attribute request.
/// </summary>
public class DeleteAttributeRequest
{
    /// <summary>
    /// The attribute id.
    /// </summary>
    /// <value>The attribute id.</value>
    public Guid Id { get; set; }
}