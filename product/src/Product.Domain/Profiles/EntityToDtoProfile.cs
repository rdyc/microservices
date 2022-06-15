using AutoMapper;
using Product.Contract.Dtos;
using Product.Domain.Converters;
using Product.Domain.Dtos;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Profiles
{
    internal class EntityToDtoProfile : Profile
    {
        public EntityToDtoProfile() : base("Entity To Dto profile")
        {
            CreateMap<ProductEntity, IProductDto>()
                .ConvertUsing<DtoConverter>();

            CreateMap<ProductEntity, ProductDto>();
        }
    }
}