using AutoMapper;
using Product.Domain.Dto;
using Product.Domain.Entity;

namespace Product.Domain.MapProfile
{
    public class EntityToDtoProfile : Profile
    {
        public EntityToDtoProfile() : base("Entity To Dto profile")
        {
            CreateMap<ItemEntity, ItemDto>();
        }
    }
}