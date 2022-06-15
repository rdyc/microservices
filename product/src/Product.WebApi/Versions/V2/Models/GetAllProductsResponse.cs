using System.Collections.Generic;

namespace Product.WebApi.Versions.V2.Models
{
    /// <summary>
    /// The api response of products.
    /// </summary>
    public class GetAllProductsResponse
    {
        /// <summary>
        /// The products.
        /// </summary>
        /// <value></value>
        public IEnumerable<ProductResponse> Products { get; set; }
    }
}