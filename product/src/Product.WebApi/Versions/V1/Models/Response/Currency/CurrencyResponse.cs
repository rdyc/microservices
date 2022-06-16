using System;
using Shared.Infrastructure.Reponses;

namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The currency response.
    /// </summary>
    public class CurrencyResponse : IApiResponse
    {
        /// <summary>
        /// The currency id.
        /// </summary>
        /// <value>The currency id.</value>
        public Guid Id { get; private set; }

        /// <summary>
        /// The currency name.
        /// </summary>
        /// <value>The currency name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// The currency description.
        /// </summary>
        /// <value>The currency description.</value>
        public string Code { get; private set; }

        /// <summary>
        /// The currency price.
        /// </summary>
        /// <value>The currency price.</value>
        public string Symbol { get; private set; }
    }
}