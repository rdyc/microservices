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

        /// <summary>
        /// The product currency.
        /// </summary>
        /// <value>The product currency.</value>
        public CurrencyResponse Currency { get; private set; }

        /// <summary>
        /// The product price.
        /// </summary>
        /// <value>The product price.</value>
        public decimal Price { get; private set; }
    }
}