using AutoMapper;
using Product.Contract.Command;
using Product.WebApi.Versions.V1.Models;

namespace Product.WebApi.Versions.V1.Profiles
{
    internal class RequestToCommandProfile : Profile
    {
        public RequestToCommandProfile() : base("Request to command profile")
        {
            CreateMap<CreateItemRequest, CreateItemCommand>()
                .ForMember(dst => dst.Id, opt => opt.Ignore());
            CreateMap<UpdateItemRequest, UpdateItemCommand>();
            CreateMap<DeleteItemRequest, DeleteItemCommand>()
                .ForMember(dst => dst.Name, opt => opt.Ignore())
                .ForMember(dst => dst.Description, opt => opt.Ignore());
        }
    }
}