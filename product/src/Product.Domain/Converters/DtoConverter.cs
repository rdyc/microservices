using AutoMapper;
using Product.Contract.Dtos;
using Product.Domain.Dtos;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Converters
{
    internal class DtoConverter :
        ITypeConverter<ProductEntity, IProductDto>
    {
        public IProductDto Convert(ProductEntity source, IProductDto destination, ResolutionContext context)
        {
            return context.Mapper.Map<ProductDto>(source);
        }
    }
}