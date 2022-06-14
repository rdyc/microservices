using AutoMapper;
using Product.WebApi.Versions.V1.Models;
using Shared.Infrastructure.Request;

namespace Product.WebApi.Versions.V1.Converters
{
    internal class PagedConverter : IValueConverter<GetAllItemsRequest, PagedQuery>
    {
        public PagedQuery Convert(GetAllItemsRequest sourceMember, ResolutionContext context)
        {
            if (sourceMember.Index.HasValue && sourceMember.Size.HasValue)
            {
                return new PagedQuery(sourceMember.Index.Value, sourceMember.Size.Value);
            }

            return null;
        }
    }
}