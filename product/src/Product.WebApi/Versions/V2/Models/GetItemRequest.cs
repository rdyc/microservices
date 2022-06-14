using System;

namespace Product.WebApi.Versions.V2.Models
{
    /// <summary>
    /// The get product item request.
    /// </summary>
    public class GetItemRequest
    {
        /// <summary>
        /// The item id.
        /// </summary>
        /// <value>The item id.</value>
        public Guid Id { get; set; }
    }
}