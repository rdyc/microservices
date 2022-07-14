using Cart.ShoppingCarts;
using Cart.ShoppingCarts.AddingProduct;
using Cart.ShoppingCarts.CancelingCart;
using Cart.ShoppingCarts.ConfirmingCart;
using Cart.ShoppingCarts.GettingCartAtVersion;
using Cart.ShoppingCarts.GettingCartById;
using Cart.ShoppingCarts.GettingCartHistory;
using Cart.ShoppingCarts.GettingCarts;
using Cart.ShoppingCarts.OpeningCart;
using Cart.ShoppingCarts.RemovingProduct;
using FW.Core.Commands;
using FW.Core.EventStoreDB.Repository;
using FW.Core.Pagination;
using FW.Core.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Carts.ShoppingCarts;

internal static class CartsConfig
{
    internal static IServiceCollection AddCarts(this IServiceCollection services) =>
        services
            .AddScoped<IEventStoreDBRepository<ShoppingCart>, EventStoreDBRepository<ShoppingCart>>()
            .AddCommandHandlers()
            .AddProjections()
            .AddQueryHandlers();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<OpenShoppingCart, HandleOpenCart>()
            .AddCommandHandler<AddProduct, HandleAddProduct>()
            .AddCommandHandler<RemoveProduct, HandleRemoveProduct>()
            .AddCommandHandler<ConfirmShoppingCart, HandleConfirmCart>()
            .AddCommandHandler<CancelShoppingCart, HandleCancelCart>();

    private static IServiceCollection AddProjections(this IServiceCollection services)
    {
        /* services
            .Project<ShoppingCartOpened, ShoppingCartDetails>(@event => @event.CartId)
            .Project<ProductAdded, ShoppingCartDetails>(@event => @event.CartId)
            .Project<ProductRemoved, ShoppingCartDetails>(@event => @event.CartId)
            .Project<ShoppingCartConfirmed, ShoppingCartDetails>(@event => @event.CartId)
            .Project<ShoppingCartCanceled, ShoppingCartDetails>(@event => @event.CartId);

        services
            .Project<ShoppingCartOpened, ShoppingCartShortInfo>(@event => @event.CartId)
            .Project<ProductAdded, ShoppingCartShortInfo>(@event => @event.CartId)
            .Project<ProductRemoved, ShoppingCartShortInfo>(@event => @event.CartId)
            .Project<ShoppingCartConfirmed, ShoppingCartShortInfo>(@event => @event.CartId)
            .Project<ShoppingCartCanceled, ShoppingCartShortInfo>(@event => @event.CartId);

        services
            .Project<ShoppingCartOpened, CartHistory>(@event => @event.CartId)
            .Project<ProductAdded, CartHistory>(@event => @event.CartId)
            .Project<ProductRemoved, CartHistory>(@event => @event.CartId)
            .Project<ShoppingCartConfirmed, CartHistory>(@event => @event.CartId)
            .Project<ShoppingCartCanceled, CartHistory>(@event => @event.CartId); */

        return services;
    }

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services
            .AddQueryHandler<GetCartById, ShoppingCartDetails, HandleGetCartById>()
            .AddQueryHandler<GetCarts, IListPaged<ShoppingCartShortInfo>, HandleGetCarts>()
            .AddQueryHandler<GetCartHistory, IListPaged<ShopppingCartHistory>, HandleGetCartHistory>()
            .AddQueryHandler<GetCartAtVersion, ShoppingCartDetails, HandleGetCartAtVersion>();
}
