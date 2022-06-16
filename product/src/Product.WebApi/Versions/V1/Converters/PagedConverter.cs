using AutoMapper;
using Product.WebApi.Versions.V1.Models;
using Shared.Infrastructure.Request;

namespace Product.WebApi.Versions.V1.Converters
{
    internal class PagedConverter :
        IValueConverter<GetAllAttributesRequest, PagedQuery>,
        IValueConverter<GetAllCurrenciesRequest, PagedQuery>,
        IValueConverter<GetAllProductsRequest, PagedQuery>
    {
        public PagedQuery Convert(GetAllAttributesRequest sourceMember, ResolutionContext context)
        {
            if (sourceMember.Index.HasValue && sourceMember.Size.HasValue)
            {
                return new PagedQuery(sourceMember.Index.Value, sourceMember.Size.Value);
            }

            return null;
        }

        public PagedQuery Convert(GetAllCurrenciesRequest sourceMember, ResolutionContext context)
        {
            if (sourceMember.Index.HasValue && sourceMember.Size.HasValue)
            {
                return new PagedQuery(sourceMember.Index.Value, sourceMember.Size.Value);
            }

            return null;
        }

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