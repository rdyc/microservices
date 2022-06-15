using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Product.WebApi.Versions.V2.Converters;
using Product.WebApi.Versions.V2.Profiles;

namespace Product.WebApi.Versions.V2
{
    internal static class ServiceExtension
    {
        public static void AddV2Service(this IServiceCollection services)
        {
            services.AddAutoMapper(config =>
            {
                config.AddProfile<DtoToResponseProfile>();
                config.AddProfile<RequestToQueryProfile>();
            });

            services.AddScoped<CriteriaConverter>();
            services.AddScoped<PagedConverter>();
            services.AddScoped<OrderedConverter>();
        }
    }
}