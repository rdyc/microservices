using FW.Core.Commands;
using FW.Core.Events;
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
            .AddEventHandlers()
            .AddProjections()
            .AddQueryHandlers();

    private static IServiceCollection AddCommandValidators(this IServiceCollection services) =>
        services
            .AddCommandValidator<RegisterProduct, ValidateRegisterProduct>()
            .AddCommandValidator<ModifyProduct, ValidateModifyProduct>()
            .AddCommandValidator<AddAttribute, ValidateAddAttribute>()
            .AddCommandValidator<RemoveAttribute, ValidateRemoveAttribute>()
            .AddCommandValidator<UpdatePrice, ValidateUpdatePrice>()
            .AddCommandValidator<UpdateStock, ValidateUpdateStock>()
            .AddCommandValidator<RemoveProduct, ValidateRemoveProduct>();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<RegisterProduct, HandleRegisterProduct>()
            .AddCommandHandler<ModifyProduct, HandleModifyProduct>()
            .AddCommandHandler<AddAttribute, HandleAddAttribute>()
            .AddCommandHandler<RemoveAttribute, HandleRemoveAttribute>()
            .AddCommandHandler<UpdatePrice, HandleUpdatePrice>()
            .AddCommandHandler<UpdateStock, HandleUpdateStock>()
            .AddCommandHandler<RemoveProduct, HandleRemoveProduct>();

    private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services
            .AddEventHandler<EventEnvelope<ProductRegistered>, HandleProductChanged>()
            .AddEventHandler<EventEnvelope<ProductModified>, HandleProductChanged>()
            .AddEventHandler<EventEnvelope<ProductRemoved>, HandleProductChanged>();

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services
            .AddQueryHandler<GetProducts, IListPaged<ProductShortInfo>, HandleGetProducts>()
            .AddQueryHandler<GetProductById, ProductDetail, HandleGetProductById>()
            .AddQueryHandler<GetProductAtVersion, Product, HandleGetProductAtVersion>()
            .AddQueryHandler<GetProductHistory, ProductHistory, HandleGetProductHistory>();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services
            .Project<ProductRegistered, ProductHistory>(
                getId: @event => @event.ProductId,
                filterBy: (productId, filter) => filter.Eq(e => e.AggregateId, productId)
            )
            .Project<ProductModified, ProductHistory>(
                getId: @event => @event.ProductId,
                filterBy: (productId, filter) => filter.Eq(e => e.AggregateId, productId)
            )
            .Project<AttributeAdded, ProductHistory>(
                getId: @event => @event.ProductId,
                filterBy: (productId, filter) => filter.Eq(e => e.AggregateId, productId)
            )
            .Project<AttributeRemoved, ProductHistory>(
                getId: @event => @event.ProductId,
                filterBy: (productId, filter) => filter.Eq(e => e.AggregateId, productId)
            )
            .Project<PriceChanged, ProductHistory>(
                getId: @event => @event.ProductId,
                filterBy: (productId, filter) => filter.Eq(e => e.AggregateId, productId)
            )
            .Project<StockChanged, ProductHistory>(
                getId: @event => @event.ProductId,
                filterBy: (productId, filter) => filter.Eq(e => e.AggregateId, productId)
            )
            .Project<ProductRemoved, ProductHistory>(
                getId: @event => @event.ProductId,
                filterBy: (productId, filter) => filter.Eq(e => e.AggregateId, productId)
            );
}