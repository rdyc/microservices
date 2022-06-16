using Product.Contract.Enums;

namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The attribute request.
    /// </summary>
    public class AttributeRequest
    {
        /// <summary>
        /// The attribute name.
        /// </summary>
        /// <value>The attribute name.</value>
        public string Name { get; set; }
        
        /// <summary>
        /// The attribute type.
        /// </summary>
        /// <value>The attribute type.</value>
        public AttributeType Type { get; set; }

        /// <summary>
        /// The attribute unit.
        /// </summary>
        /// <value>The attribute unit.</value>
        public string Unit { get; set; }
    }
}