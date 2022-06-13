// This source code (Finder.cs) is Copyright © PT. Xsis Mitra Utama.
// You MAY NOT copied, reproduced, published, distributed or transmitted
// to or stored in any manner without priorwritten consent from PT. Xsis Mitra Utama.

using System;
using System.Collections.Generic;

namespace Shared.Infrastructure.Request
{
    public class CriteriaQuery<T>
        where T : struct, IConvertible
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CriteriaQuery{T}"/> class.
        /// </summary>
        public CriteriaQuery()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException($"The type of{typeof(T)} must be an enumerated type");
            }
        }

        private IList<CriteriaQueryValue<T>> values { get; set; }

        public IEnumerable<CriteriaQueryValue<T>> Values => values;

        public void AddValues(T field, object value)
        {
            if (values == null)
            {
                values = new List<CriteriaQueryValue<T>>();
            }

            values.Add(new CriteriaQueryValue<T>(field, value));
        }
    }

    public class CriteriaQueryValue<T>
        where T : struct, IConvertible
    {
        public CriteriaQueryValue(T field, object value)
        {
            Field = field;
            Value = value;
        }

        public T Field { get; private set; }

        public object Value { get; private set; }
    }
}