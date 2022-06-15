using System.Collections.Generic;

namespace Shared.Infrastructure.Reponses
{
    public class ListPagedResponse<T>
        where T : IApiResponse
    {
        public IEnumerable<T> Data { get; private set; }
    }
}