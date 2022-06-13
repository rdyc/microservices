// This source code (Paging.cs) is Copyright © PT. Xsis Mitra Utama.
// You MAY NOT copied, reproduced, published, distributed or transmitted
// to or stored in any manner without priorwritten consent from PT. Xsis Mitra Utama.

namespace Shared.Infrastructure.Request
{
    /// <summary>
    /// The paged query request.
    /// </summary>
    public class PagedQuery
    {
        private int index = 0;

        public PagedQuery(int index, int size)
        {
            Index = index;
            Size = size;
        }

        /// <summary>
        /// Gets or sets page number (def=0).
        /// </summary>
        /// <value>
        /// The page index.
        /// </value>
        public int Index
        {
            get => index;
            set
            {
                index = value < 0 ? 0 : value;
            }
        }

        /// <summary>
        /// Gets or sets size limit (def=10).
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public int Size { get; private set; }
    }
}