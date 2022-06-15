using System.Collections.Generic;
using AutoMapper;
using Product.Contract.Dtos;
using Product.WebApi.Versions.V1.Models;

namespace Product.WebApi.Versions.V1.Profiles
{
    internal class DtoToResponseProfile : Profile
    {
        public DtoToResponseProfile() : base("V1 Dto to response profile")
        {
            CreateMap<IEnumerable<IProductDto>, GetAllProductsResponse>()
                .ForMember(dst => dst.Data, opt => opt.MapFrom(src => src));

            CreateMap<IProductDto, ProductResponse>();
        }
    }
}