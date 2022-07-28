using FW.Core.MongoDB.Projections;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Cart.Products;

public static class ProductServices
{
    public static IServiceCollection AddProduct(this IServiceCollection services) =>
        services.AddProjections();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services
            .Projection<Product>(builder => builder
                .AddOn<ProductRegistered>(ProductProjection.Handle)
                .UpdateOn<ProductModified>(
                    onGet: e => e.Id,
                    onHandle: ProductProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Sku, view.Sku)
                        .Set(e => e.Name, view.Name)
                        .Set(e => e.Description, view.Description)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductAttributeAdded>(
                    onGet: e => e.Id,
                    onHandle: ProductProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Attributes, view.Attributes)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductAttributeRemoved>(
                    onGet: e => e.Id,
                    onHandle: ProductProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Attributes, view.Attributes)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductPriceChanged>(
                    onGet: e => e.Id,
                    onHandle: ProductProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Currency, view.Currency)
                        .Set(e => e.Price, view.Price)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductStockChanged>(
                    onGet: e => e.Id,
                    onHandle: ProductProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Stock, view.Stock)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductShipped>(
                    onGet: e => e.ProductId,
                    onHandle: ProductProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Stock, view.Stock)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductRemoved>(
                    onGet: e => e.Id,
                    onHandle: ProductProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            );
}