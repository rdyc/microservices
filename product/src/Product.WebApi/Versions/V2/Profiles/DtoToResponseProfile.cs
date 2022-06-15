using System.Collections.Generic;
using AutoMapper;
using Product.Contract.Dtos;
using Product.WebApi.Versions.V2.Models;

namespace Product.WebApi.Versions.V2.Profiles
{
    internal class DtoToResponseProfile : Profile
    {
        public DtoToResponseProfile() : base("V2 Dto to response profile")
        {
            CreateMap<IEnumerable<IProductDto>, GetAllProductsResponse>()
                .ForMember(dst => dst.Products, opt => opt.MapFrom(src => src));

            CreateMap<IProductDto, ProductResponse>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => $"{src.Name}, {src.Description}"));
        }
    }
}