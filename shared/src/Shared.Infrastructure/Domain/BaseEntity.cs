namespace Shared.Infrastructure.Domain
{
    public class BaseEntity
    {
        /// <summary>
        /// Gets or sets a value indicating whether gets the entity status.
        /// </summary>
        /// <value>
        /// The entity status.
        /// </value>
        public bool IsTransient { get; set; }
    }
}