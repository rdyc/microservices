using AutoMapper;
using Product.Contract.Dtos;
using Product.Domain.Dtos;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Converters
{
    internal class DtoConverter :
        ITypeConverter<AttributeEntity, IAttributeDto>,
        ITypeConverter<CurrencyEntity, ICurrencyDto>,
        ITypeConverter<ProductEntity, IProductDto>,
        ITypeConverter<CurrencyReferenceEntity, ICurrencyDto>
    {
        public IAttributeDto Convert(AttributeEntity source, IAttributeDto destination, ResolutionContext context)
        {
            return context.Mapper.Map<AttributeDto>(source);
        }

        public ICurrencyDto Convert(CurrencyEntity source, ICurrencyDto destination, ResolutionContext context)
        {
            return context.Mapper.Map<CurrencyDto>(source);
        }

        public IProductDto Convert(ProductEntity source, IProductDto destination, ResolutionContext context)
        {
            return context.Mapper.Map<ProductDto>(source);
        }

        public ICurrencyDto Convert(CurrencyReferenceEntity source, ICurrencyDto destination, ResolutionContext context)
        {
            return context.Mapper.Map<CurrencyDto>(source);
        }
    }
}