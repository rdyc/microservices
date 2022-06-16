using System;
using Product.Contract.Enums;
using Shared.Infrastructure.Reponses;

namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The attribute response.
    /// </summary>
    public class AttributeResponse : IApiResponse
    {
        /// <summary>
        /// The attribute id.
        /// </summary>
        /// <value>The attribute id.</value>
        public Guid Id { get; private set; }

        /// <summary>
        /// The attribute name.
        /// </summary>
        /// <value>The attribute name.</value>
        public string Name { get; private set; }
        
        /// <summary>
        /// The attribute type.
        /// </summary>
        /// <value>The attribute type.</value>
        public AttributeType Type { get; private set; }

        /// <summary>
        /// The attribute unit.
        /// </summary>
        /// <value>The attribute unit.</value>
        public string Unit { get; private set; }
    }
}