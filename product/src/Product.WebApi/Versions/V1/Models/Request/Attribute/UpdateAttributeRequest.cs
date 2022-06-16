using System;
using System.Text.Json.Serialization;

namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The update attribute request.
    /// </summary>
    public class UpdateAttributeRequest : AttributeRequest
    {
        /// <summary>
        /// The attribute id.
        /// </summary>
        /// <value>The attribute id.</value>
        [JsonIgnore]
        public Guid Id { get; set; }
    }
}