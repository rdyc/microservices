using System;

namespace Product.WebApi.Versions.V2.Models
{
    /// <summary>
    /// The get product request.
    /// </summary>
    public class GetProductRequest
    {
        /// <summary>
        /// The product id.
        /// </summary>
        /// <value>The product id.</value>
        public Guid Id { get; set; }
    }
}