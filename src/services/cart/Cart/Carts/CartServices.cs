using Cart.Carts.AddingProduct;
using Cart.Carts.CancelingCart;
using Cart.Carts.ConfirmingCart;
using Cart.Carts.FinalizingCart;
using Cart.Carts.GettingCartAtVersion;
using Cart.Carts.GettingCartById;
using Cart.Carts.GettingCartHistory;
using Cart.Carts.GettingCarts;
using Cart.Carts.OpeningCart;
using Cart.Carts.RemovingProduct;
using FW.Core.Commands;
using FW.Core.Events;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB.Projections;
using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.Validation;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Cart.Carts;

internal static class CartsServices
{
    internal static IServiceCollection AddCart(this IServiceCollection services) =>
        services
            .AddScoped<IEventStoreDBRepository<Cart>, EventStoreDBRepository<Cart>>()
            .AddCommandValidators()
            .AddCommandHandlers()
            .AddQueryHandlers()
            .AddEventHandlers()
            .AddProjections();

    private static IServiceCollection AddCommandValidators(this IServiceCollection services) =>
        services
            .AddCommandValidator<OpenCart, ValidateOpenCart>()
            .AddCommandValidator<AddProductCart, ValidateAddProductCart>()
            .AddCommandValidator<RemoveProductCart, ValidateRemoveProductCart>()
            .AddCommandValidator<ConfirmCart, ValidateConfirmCart>()
            .AddCommandValidator<CancelCart, ValidateCancelCart>();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<OpenCart, HandleOpenCart>()
            .AddCommandHandler<AddProductCart, HandleAddProductCart>()
            .AddCommandHandler<RemoveProductCart, HandleRemoveProductCart>()
            .AddCommandHandler<ConfirmCart, HandleConfirmCart>()
            .AddCommandHandler<CancelCart, HandleCancelCart>();

    private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services
            .AddEventHandler<EventEnvelope<CartConfirmed>, HandleCartFinalized>();

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services
            .AddQueryHandler<GetCartById, CartDetails, HandleGetCartById>()
            .AddQueryHandler<GetCarts, IListPaged<CartShortInfo>, HandleGetCarts>()
            .AddQueryHandler<GetCartHistory, IListPaged<CartHistory>, HandleGetCartHistory>()
            .AddQueryHandler<GetCartAtVersion, Cart, HandleGetCartAtVersion>();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services
            .Projection<CartShortInfo>(builder => builder
                .AddOn<CartOpened>(CartShortInfoProjection.Handle)
                .UpdateOn<CartProductAdded>(
                    onGet: e => e.CartId,
                    onHandle: CartShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.TotalItemsCount, view.TotalItemsCount)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<CartProductRemoved>(
                    onGet: e => e.CartId,
                    onHandle: CartShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.TotalItemsCount, view.TotalItemsCount)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<CartConfirmed>(
                    onGet: e => e.CartId,
                    onHandle: CartShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.ConfirmedAt, view.ConfirmedAt)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<CartCanceled>(
                    onGet: e => e.CartId,
                    onHandle: CartShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.CanceledAt, view.CanceledAt)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            )
            .Projection<CartDetails>(builder => builder
                .AddOn<CartOpened>(CartDetailsProjection.Handle)
                .UpdateOn<CartProductAdded>(
                    onGet: e => e.CartId,
                    onHandle: CartDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Products, view.Products)
                        .Set(e => e.TotalPrice, view.Products.Sum(pi => pi.TotalPrice))
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<CartProductRemoved>(
                    onGet: e => e.CartId,
                    onHandle: CartDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Products, view.Products)
                        .Set(e => e.TotalPrice, view.Products.Sum(pi => pi.TotalPrice))
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<CartConfirmed>(
                    onGet: e => e.CartId,
                    onHandle: CartDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.ConfirmedAt, view.ConfirmedAt)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<CartCanceled>(
                    onGet: e => e.CartId,
                    onHandle: CartDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.CanceledAt, view.CanceledAt)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            )
            .Projection<CartHistory>(builder => builder
                .AddOn<CartOpened>(CartHistoryProjection.Handle)
                .AddOn<CartProductAdded>(CartHistoryProjection.Handle)
                .AddOn<CartProductRemoved>(CartHistoryProjection.Handle)
                .AddOn<CartConfirmed>(CartHistoryProjection.Handle)
                .AddOn<CartCanceled>(CartHistoryProjection.Handle)
            );
}