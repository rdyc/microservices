using FW.Core.Commands;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB.Projections;
using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.Validation;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Store.Products.AddingAttribute;
using Store.Products.GettingProductAtVersion;
using Store.Products.GettingProductById;
using Store.Products.GettingProductHistory;
using Store.Products.GettingProducts;
using Store.Products.ModifyingProduct;
using Store.Products.RegisteringProduct;
using Store.Products.RemovingAttribute;
using Store.Products.RemovingProduct;
using Store.Products.SellingProduct;
using Store.Products.UpdatingPrice;
using Store.Products.UpdatingStock;

namespace Store.Products;

public static class ProductServices
{
    public static IServiceCollection AddProduct(this IServiceCollection services) =>
        services
            .AddScoped<IEventStoreDBRepository<Product>, EventStoreDBRepository<Product>>()
            .AddCommandValidators()
            .AddCommandHandlers()
            .AddQueryHandlers()
            .AddProjections();

    private static IServiceCollection AddCommandValidators(this IServiceCollection services) =>
        services
            .AddCommandValidator<RegisterProduct, ValidateRegisterProduct>()
            .AddCommandValidator<ModifyProduct, ValidateModifyProduct>()
            .AddCommandValidator<AddProductAttribute, ValidateAddAttribute>()
            .AddCommandValidator<RemoveProductAttribute, ValidateRemoveProductAttribute>()
            .AddCommandValidator<UpdateProductPrice, ValidateUpdateProductPrice>()
            .AddCommandValidator<UpdateProductStock, ValidateUpdateProductStock>()
            .AddCommandValidator<RemoveProduct, ValidateRemoveProduct>();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<RegisterProduct, HandleRegisterProduct>()
            .AddCommandHandler<ModifyProduct, HandleModifyProduct>()
            .AddCommandHandler<AddProductAttribute, HandleAddProductAttribute>()
            .AddCommandHandler<RemoveProductAttribute, HandleRemoveAttribute>()
            .AddCommandHandler<UpdateProductPrice, HandleUpdateProductPrice>()
            .AddCommandHandler<UpdateProductStock, HandleUpdateProductStock>()
            .AddCommandHandler<RemoveProduct, HandleRemoveProduct>();

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services
            .AddQueryHandler<GetProducts, IListPaged<ProductShortInfo>, HandleGetProducts>()
            .AddQueryHandler<GetProductById, ProductDetail, HandleGetProductById>()
            .AddQueryHandler<GetProductAtVersion, Product, HandleGetProductAtVersion>()
            .AddQueryHandler<GetProductHistory, IListPaged<ProductHistory>, HandleGetProductHistory>();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services
            .Projection<ProductShortInfo>(builder => builder
                .AddOn<ProductRegistered>(ProductShortInfoProjection.Handle)
                .UpdateOn<ProductModified>(
                    onGet: e => e.Id,
                    onHandle: ProductShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Sku, view.Sku)
                        .Set(e => e.Name, view.Name)
                        .Set(e => e.Description, view.Description)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductPriceChanged>(
                    onGet: e => e.Id,
                    onHandle: ProductShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Currency, view.Currency)
                        .Set(e => e.Price, view.Price)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductStockChanged>(
                    onGet: e => e.Id,
                    onHandle: ProductShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Stock, view.Stock)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductSold>(
                    onGet: e => e.Id,
                    onHandle: ProductShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Sold, view.Sold)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductRemoved>(
                    onGet: e => e.Id,
                    onHandle: ProductShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            )
            .Projection<ProductDetail>(builder => builder
                .AddOn<ProductRegistered>(ProductDetailProjection.Handle)
                .UpdateOn<ProductModified>(
                    onGet: e => e.Id,
                    onHandle: ProductDetailProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Sku, view.Sku)
                        .Set(e => e.Name, view.Name)
                        .Set(e => e.Description, view.Description)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductAttributeAdded>(
                    onGet: e => e.Id,
                    onHandle: ProductDetailProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Attributes, view.Attributes)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductAttributeRemoved>(
                    onGet: e => e.Id,
                    onHandle: ProductDetailProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Attributes, view.Attributes)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductPriceChanged>(
                    onGet: e => e.Id,
                    onHandle: ProductDetailProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Currency, view.Currency)
                        .Set(e => e.Price, view.Price)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductStockChanged>(
                    onGet: e => e.Id,
                    onHandle: ProductDetailProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Stock, view.Stock)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductSold>(
                    onGet: e => e.Id,
                    onHandle: ProductDetailProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Sold, view.Sold)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductRemoved>(
                    onGet: e => e.Id,
                    onHandle: ProductDetailProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            )
            .Projection<ProductHistory>(builder => builder
                .AddOn<ProductRegistered>(ProductHistoryProjection.Handle)
                .AddOn<ProductModified>(ProductHistoryProjection.Handle)
                .AddOn<ProductAttributeAdded>(ProductHistoryProjection.Handle)
                .AddOn<ProductAttributeRemoved>(ProductHistoryProjection.Handle)
                .AddOn<ProductPriceChanged>(ProductHistoryProjection.Handle)
                .AddOn<ProductStockChanged>(ProductHistoryProjection.Handle)
                .AddOn<ProductSold>(ProductHistoryProjection.Handle)
                .AddOn<ProductRemoved>(ProductHistoryProjection.Handle)
            );
}