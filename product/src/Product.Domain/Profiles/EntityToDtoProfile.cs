using AutoMapper;
using Product.Contract.Dtos;
using Product.Domain.Converters;
using Product.Domain.Dtos;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Profiles;

internal class EntityToDtoProfile : Profile
{
    public EntityToDtoProfile() : base("Entity To Dto profile")
    {
        // attribute
        CreateMap<AttributeEntity, IAttributeDto>()
            .ConvertUsing<DtoConverter>();
        CreateMap<AttributeEntity, AttributeDto>();

        // currency
        CreateMap<CurrencyEntity, ICurrencyDto>()
            .ConvertUsing<DtoConverter>();
        CreateMap<CurrencyEntity, CurrencyDto>();

        // product
        CreateMap<ProductEntity, IProductDto>()
            .ConvertUsing<DtoConverter>();
        CreateMap<ProductEntity, ProductDto>();

        // reference
        CreateMap<CurrencyReferenceEntity, ICurrencyDto>()
            .ConvertUsing<DtoConverter>();
        CreateMap<CurrencyReferenceEntity, CurrencyDto>();
    }
}