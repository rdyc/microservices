using AutoMapper;
using Product.Contract.Command;
using Product.WebApi.Model;

namespace Product.WebApi
{
    public class RequestToCommandProfile : Profile
    {
        public RequestToCommandProfile() : base("Request to command profile")
        {
            CreateMap<CreateItemRequest, CreateItemCommand>()
                .ForMember(dst => dst.Id, opt => opt.Ignore());
            CreateMap<UpdateItemRequest, UpdateItemCommand>();
            CreateMap<DeleteItemRequest, DeleteItemCommand>();
        }
    }
}