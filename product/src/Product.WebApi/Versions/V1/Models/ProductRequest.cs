namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The product request.
    /// </summary>
    public class ProductRequest
    {
        /// <summary>
        /// The product name.
        /// </summary>
        /// <value>The product name.</value>
        public string Name { get; set; }
        
        /// <summary>
        /// The product description.
        /// </summary>
        /// <value>The product description.</value>
        public string Description { get; set; }
    }
}