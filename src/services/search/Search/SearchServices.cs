using FW.Core.ElasticSearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Search.Products;

namespace Search;

public static class SearchServices
{
    public static IServiceCollection AddSearchServices(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddElasticsearch(configuration, settings =>
            {
                settings.DefaultMappingFor<Product>(m => m
                    .PropertyName(p => p.Id, "id")
                    .PropertyName(p => p.Sku, "sku")
                    .PropertyName(p => p.Name, "name")
                    .PropertyName(p => p.Description, "description")
                    .PropertyName(p => p.Status, "status")
                    .PropertyName(p => p.Version, "version")
                );
            })
            .AddProduct();
}