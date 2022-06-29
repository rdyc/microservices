using AutoMapper;
using Product.Contract.Commands;
using Product.WebApi.Versions.V1.Models;

namespace Product.WebApi.Versions.V1.Profiles;

internal class RequestToCommandProfile : Profile
{
    public RequestToCommandProfile() : base("V1 Request to command profile")
    {
        // attribute
        CreateMap<CreateAttributeRequest, CreateAttributeCommand>()
            .ForMember(dst => dst.Id, opt => opt.Ignore());
        CreateMap<UpdateAttributeRequest, UpdateAttributeCommand>();
        CreateMap<DeleteAttributeRequest, DeleteAttributeCommand>()
            .ForMember(dst => dst.Name, opt => opt.Ignore())
            .ForMember(dst => dst.Type, opt => opt.Ignore())
            .ForMember(dst => dst.Unit, opt => opt.Ignore());

        // currency
        CreateMap<CreateCurrencyRequest, CreateCurrencyCommand>()
            .ForMember(dst => dst.Id, opt => opt.Ignore());
        CreateMap<UpdateCurrencyRequest, UpdateCurrencyCommand>();
        CreateMap<DeleteCurrencyRequest, DeleteCurrencyCommand>()
            .ForMember(dst => dst.Name, opt => opt.Ignore())
            .ForMember(dst => dst.Code, opt => opt.Ignore())
            .ForMember(dst => dst.Symbol, opt => opt.Ignore());

        // product
        CreateMap<CreateProductRequest, CreateProductCommand>()
            .ForMember(dst => dst.Id, opt => opt.Ignore());
        CreateMap<UpdateProductRequest, UpdateProductCommand>();
        CreateMap<DeleteProductRequest, DeleteProductCommand>()
            .ForMember(dst => dst.Name, opt => opt.Ignore())
            .ForMember(dst => dst.Description, opt => opt.Ignore())
            .ForMember(dst => dst.CurrencyId, opt => opt.Ignore())
            .ForMember(dst => dst.Price, opt => opt.Ignore());
    }
}