using System;

namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The delete product request.
    /// </summary>
    public class DeleteProductRequest
    {
        /// <summary>
        /// The product id.
        /// </summary>
        /// <value>The product id.</value>
        public Guid Id { get; set; }
    }
}