using System;

namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The get attribute request.
    /// </summary>
    public class GetAttributeRequest
    {
        /// <summary>
        /// The attribute id.
        /// </summary>
        /// <value>The attribute id.</value>
        public Guid Id { get; set; }
    }
}