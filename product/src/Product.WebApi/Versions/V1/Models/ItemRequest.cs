namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The product item request.
    /// </summary>
    public class ItemRequest
    {
        /// <summary>
        /// The item name.
        /// </summary>
        /// <value>The item name.</value>
        public string Name { get; set; }
        
        /// <summary>
        /// The item description.
        /// </summary>
        /// <value>The item description.</value>
        public string Description { get; set; }
    }
}