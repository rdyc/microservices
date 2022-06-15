using AutoMapper;
using Product.WebApi.Versions.V2.Models;
using Shared.Infrastructure.Request;

namespace Product.WebApi.Versions.V2.Converters
{
    internal class PagedConverter : IValueConverter<GetAllProductsRequest, PagedQuery>
    {
        public PagedQuery Convert(GetAllProductsRequest sourceMember, ResolutionContext context)
        {
            if (sourceMember.Index.HasValue && sourceMember.Size.HasValue)
            {
                return new PagedQuery(sourceMember.Index.Value, sourceMember.Size.Value);
            }

            return null;
        }
    }
}