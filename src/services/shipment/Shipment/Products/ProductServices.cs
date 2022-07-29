using Cart.Products.UpdatingStock;
using FW.Core.MongoDB.Projections;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Shipment.Products.ModifyingProduct;
using Shipment.Products.RegisteringProduct;
using Shipment.Products.RemovingProduct;
using Shipment.Products.ShippingProduct;

namespace Shipment.Products;

public static class ProductServices
{
    public static IServiceCollection AddProduct(this IServiceCollection services) =>
        services.AddProjections();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services
            .Projection<Product>(builder => builder
                .AddOn<ProductRegistered>(ProductProjection.Handle)
                .UpdateOn<ProductModified>(
                    onGet: e => e.ProductId,
                    onHandle: ProductProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Sku, view.Sku)
                        .Set(e => e.Name, view.Name)
                        .Set(e => e.Description, view.Description)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductStockChanged>(
                    onGet: e => e.ProductId,
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
                    onGet: e => e.ProductId,
                    onHandle: ProductProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            );
}