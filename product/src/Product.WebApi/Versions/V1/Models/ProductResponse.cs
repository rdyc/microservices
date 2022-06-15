using System;
using Shared.Infrastructure.Reponses;

namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The product response.
    /// </summary>
    public class ProductResponse : IApiResponse
    {
        /// <summary>
        /// The product id.
        /// </summary>
        /// <value>The product id.</value>
        public Guid Id { get; private set; }

        /// <summary>
        /// The product name.
        /// </summary>
        /// <value>The product name.</value>
        public string Name { get; private set; }
        
        /// <summary>
        /// The product description.
        /// </summary>
        /// <value>The product description.</value>
        public string Description { get; private set; }
    }
}