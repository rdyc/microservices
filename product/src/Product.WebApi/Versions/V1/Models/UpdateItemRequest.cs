using System;
using System.Text.Json.Serialization;

namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The update product item request.
    /// </summary>
    public class UpdateItemRequest : ItemRequest
    {
        /// <summary>
        /// The item id.
        /// </summary>
        /// <value>The item id.</value>
        [JsonIgnore]
        public Guid Id { get; set; }
    }
}