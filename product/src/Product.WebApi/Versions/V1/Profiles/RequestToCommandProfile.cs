using AutoMapper;
using Product.Contract.Commands;
using Product.WebApi.Versions.V1.Models;

namespace Product.WebApi.Versions.V1.Profiles
{
    internal class RequestToCommandProfile : Profile
    {
        public RequestToCommandProfile() : base("V1 Request to command profile")
        {
            CreateMap<CreateProductRequest, CreateProductCommand>()
                .ForMember(dst => dst.Id, opt => opt.Ignore());
            CreateMap<UpdateProductRequest, UpdateProductCommand>();
            CreateMap<DeleteProductRequest, DeleteProductCommand>()
                .ForMember(dst => dst.Name, opt => opt.Ignore())
                .ForMember(dst => dst.Description, opt => opt.Ignore());
        }
    }
}