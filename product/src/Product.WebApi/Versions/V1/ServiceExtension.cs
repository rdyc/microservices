using Microsoft.Extensions.DependencyInjection;
using Product.WebApi.Versions.V1.Converters;
using Product.WebApi.Versions.V1.Profiles;

namespace Product.WebApi.Versions.V1;

internal static class ServiceExtension
{
    public static IServiceCollection AddV1Service(this IServiceCollection services) =>
        services
            .AddAutoMapper(config =>
            {
                config.AddProfile<DtoToResponseProfile>();
                config.AddProfile<RequestToCommandProfile>();
                config.AddProfile<RequestToQueryProfile>();
            })
            .AddScoped<CriteriaConverter>()
            .AddScoped<PagedConverter>()
            .AddScoped<OrderedConverter>();
}