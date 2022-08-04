using Cart.Carts;
using Cart.Carts.OpeningCart;
using FW.Core.Testing;
using FluentAssertions;

namespace Cart.Tests.Extensions.Carts;

internal static class CartExtensions
{
    public static Cart.Carts.Cart IsOpenedCartWith(
        this Cart.Carts.Cart shoppingCart,
        Guid id,
        Guid clientId)
    {

        shoppingCart.Id.Should().Be(id);
        shoppingCart.ClientId.Should().Be(clientId);
        shoppingCart.Status.Should().Be(CartStatus.Pending);
        shoppingCart.ProductItems.Should().BeEmpty();
        shoppingCart.TotalPrice.Should().Be(0);

        return shoppingCart;
    }

    public static Cart.Carts.Cart HasCartOpenedEventWith(
        this Cart.Carts.Cart shoppingCart,
        Guid id,
        Guid clientId)
    {
        var @event = shoppingCart.PublishedEvent<CartOpened>();

        @event.Should().NotBeNull();
        @event.Should().BeOfType<CartOpened>();
        @event!.CartId.Should().Be(id);
        @event.ClientId.Should().Be(clientId);

        return shoppingCart;
    }
}
