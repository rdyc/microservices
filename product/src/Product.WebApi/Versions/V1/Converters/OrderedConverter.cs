using AutoMapper;
using Product.Contract.Queries;
using Product.WebApi.Versions.V1.Models;
using Shared.Infrastructure.Request;

namespace Product.WebApi.Versions.V1.Converters
{
    internal class OrderedConverter :
        IValueConverter<GetAllAttributesRequest, OrderedQuery<AttributeField>>,
        IValueConverter<GetAllCurrenciesRequest, OrderedQuery<CurrencyField>>,
        IValueConverter<GetAllProductsRequest, OrderedQuery<ProductField>>
    {
        public OrderedQuery<AttributeField> Convert(GetAllAttributesRequest sourceMember, ResolutionContext context)
        {
            return null;
        }

        public OrderedQuery<CurrencyField> Convert(GetAllCurrenciesRequest sourceMember, ResolutionContext context)
        {
            return null;
        }

        public OrderedQuery<ProductField> Convert(GetAllProductsRequest sourceMember, ResolutionContext context)
        {
            // if (sourceMember.Index.HasValue && sourceMember.Size.HasValue)
            // {
            //     return new PagedQuery(sourceMember.Index.Value, sourceMember.Size.Value);
            // }

            return null;
        }
    }
}