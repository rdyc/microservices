using FW.Core.ElasticSearch.Repository;
using FW.Core.Events;
using FW.Core.Queries;
using Microsoft.Extensions.DependencyInjection;
using Search.Products.ModifyingProduct;
using Search.Products.RegisteringProduct;
using Search.Products.RemovingProduct;
using Search.Products.SearchingProducts;

namespace Search.Products;

public static class Config
{
    public static IServiceCollection AddProduct(this IServiceCollection services) =>
        services
            .AddScoped<IElasticSearchRepository<Product>, ElasticSearchRepository<Product>>()
            .AddEventHandler<ProductRegistered, HandleProductChanged>()
            .AddEventHandler<ProductModified, HandleProductChanged>()
            .AddEventHandler<ProductRemoved, HandleProductChanged>()
            .AddQueryHandler<SearchProducts, IReadOnlyCollection<Product>, HandleSearchProducts>();
}
