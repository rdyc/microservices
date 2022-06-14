using System;

namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The delete product item request.
    /// </summary>
    public class DeleteItemRequest
    {
        /// <summary>
        /// The item id.
        /// </summary>
        /// <value>The item id.</value>
        public Guid Id { get; set; }
    }
}