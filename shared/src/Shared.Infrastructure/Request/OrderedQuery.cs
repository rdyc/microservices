// This source code (Order.cs) is Copyright © PT. Xsis Mitra Utama.
// You MAY NOT copied, reproduced, published, distributed or transmitted
// to or stored in any manner without priorwritten consent from PT. Xsis Mitra Utama.

using System;
using Shared.Infrastructure.Enums;

namespace Shared.Infrastructure.Request
{
    /// <summary>
    /// The orderable query request.
    /// </summary>
    /// <typeparam name="T">The type of ordered fields.</typeparam>
    public class OrderedQuery<T>
        where T : struct, IConvertible
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Order{T}"/> class.
        /// </summary>
        public OrderedQuery()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException($"{nameof(T)} must be an enumerated type");
            }
        }
        /// <summary>
        /// Gets or sets order by field.
        /// </summary>
        /// <value>
        /// The order field.
        /// </value>
        public T Field { get; set; }

        /// <summary>
        /// Gets or sets order sort.
        /// </summary>
        /// <value>
        /// The order sort.
        /// </value>
        public Direction Direction { get; set; }
    }
}