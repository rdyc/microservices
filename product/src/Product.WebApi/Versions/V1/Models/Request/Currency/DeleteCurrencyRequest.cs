using System;

namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The delete currency request.
    /// </summary>
    public class DeleteCurrencyRequest
    {
        /// <summary>
        /// The currency id.
        /// </summary>
        /// <value>The currency id.</value>
        public Guid Id { get; set; }
    }
}