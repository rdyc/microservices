using AutoMapper;
using Product.WebApi.Model;
using Shared.Infrastructure.Request;

namespace Product.WebApi.Converters
{
    public class PagedConverter : IValueConverter<GetAllItemsRequest, PagedQuery>
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