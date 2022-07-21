using Cart.ShoppingCarts;
using Cart.ShoppingCarts.AddingProduct;
using Cart.ShoppingCarts.CancelingCart;
using Cart.ShoppingCarts.ConfirmingCart;
using Cart.ShoppingCarts.FinalizingCart;
using Cart.ShoppingCarts.GettingCartAtVersion;
using Cart.ShoppingCarts.GettingCartById;
using Cart.ShoppingCarts.GettingCartHistory;
using Cart.ShoppingCarts.GettingCarts;
using Cart.ShoppingCarts.OpeningCart;
using Cart.ShoppingCarts.RemovingProduct;
using FW.Core.Commands;
using FW.Core.Events;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB.Projections;
using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.Validation;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Carts.ShoppingCarts;

internal static class ShoppingCartsServices
{
    internal static IServiceCollection AddCart(this IServiceCollection services) =>
        services
            .AddScoped<IEventStoreDBRepository<ShoppingCart>, EventStoreDBRepository<ShoppingCart>>()
            .AddCommandValidators()
            .AddCommandHandlers()
            .AddQueryHandlers()
            .AddEventHandlers()
            .AddProjections();

    private static IServiceCollection AddCommandValidators(this IServiceCollection services) =>
        services
            .AddCommandValidator<OpenShoppingCart, ValidateOpenShoppingCart>()
            .AddCommandValidator<AddProductCart, ValidateAddProductCart>()
            .AddCommandValidator<RemoveProductCart, ValidateRemoveProductCart>()
            .AddCommandValidator<ConfirmShoppingCart, ValidateConfirmShoppingCart>()
            .AddCommandValidator<CancelShoppingCart, ValidateCancelShoppingCart>();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<OpenShoppingCart, HandleOpenCart>()
            .AddCommandHandler<AddProductCart, HandleAddProductCart>()
            .AddCommandHandler<RemoveProductCart, HandleRemoveProductCart>()
            .AddCommandHandler<ConfirmShoppingCart, HandleConfirmCart>()
            .AddCommandHandler<CancelShoppingCart, HandleCancelCart>();

    private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services
            .AddEventHandler<EventEnvelope<ShoppingCartConfirmed>, HandleShoppingCartFinalized>();

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services
            .AddQueryHandler<GetCartById, ShoppingCartDetails, HandleGetCartById>()
            .AddQueryHandler<GetCarts, IListPaged<ShoppingCartShortInfo>, HandleGetCarts>()
            .AddQueryHandler<GetCartHistory, IListPaged<ShoppingCartHistory>, HandleGetCartHistory>()
            .AddQueryHandler<GetCartAtVersion, ShoppingCart, HandleGetCartAtVersion>();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services
            .Projection<ShoppingCartShortInfo>(builder => builder
                .AddOn<ShoppingCartOpened>(ShoppingCartShortInfoProjection.Handle)
                .UpdateOn<ProductCartAdded>(
                    onGet: e => e.CartId,
                    onHandle: ShoppingCartShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.TotalItemsCount, view.TotalItemsCount)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductCartRemoved>(
                    onGet: e => e.CartId,
                    onHandle: ShoppingCartShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.TotalItemsCount, view.TotalItemsCount)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ShoppingCartConfirmed>(
                    onGet: e => e.CartId,
                    onHandle: ShoppingCartShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.ConfirmedAt, view.ConfirmedAt)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ShoppingCartCanceled>(
                    onGet: e => e.CartId,
                    onHandle: ShoppingCartShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.CanceledAt, view.CanceledAt)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            )
            .Projection<ShoppingCartDetails>(builder => builder
                .AddOn<ShoppingCartOpened>(ShoppingCartDetailsProjection.Handle)
                .UpdateOn<ProductCartAdded>(
                    onGet: e => e.CartId,
                    onHandle: ShoppingCartDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Products, view.Products)
                        .Set(e => e.TotalPrice, view.Products.Sum(pi => pi.TotalPrice))
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ProductCartRemoved>(
                    onGet: e => e.CartId,
                    onHandle: ShoppingCartDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Products, view.Products)
                        .Set(e => e.TotalPrice, view.Products.Sum(pi => pi.TotalPrice))
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ShoppingCartConfirmed>(
                    onGet: e => e.CartId,
                    onHandle: ShoppingCartDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.ConfirmedAt, view.ConfirmedAt)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<ShoppingCartCanceled>(
                    onGet: e => e.CartId,
                    onHandle: ShoppingCartDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.CanceledAt, view.CanceledAt)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            )
            .Projection<ShoppingCartHistory>(builder => builder
                .AddOn<ShoppingCartOpened>(ShoppingCartHistoryProjection.Handle)
                .AddOn<ProductCartAdded>(ShoppingCartHistoryProjection.Handle)
                .AddOn<ProductCartRemoved>(ShoppingCartHistoryProjection.Handle)
                .AddOn<ShoppingCartConfirmed>(ShoppingCartHistoryProjection.Handle)
                .AddOn<ShoppingCartCanceled>(ShoppingCartHistoryProjection.Handle)
            );
}