using System;
using System.Text.Json.Serialization;

namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The update product request.
    /// </summary>
    public class UpdateProductRequest : ProductRequest
    {
        /// <summary>
        /// The product id.
        /// </summary>
        /// <value>The product id.</value>
        [JsonIgnore]
        public Guid Id { get; set; }
    }
}