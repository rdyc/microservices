using Cart.Carts;
using Cart.Carts.OpeningCart;
using FW.Core.Testing;
using FluentAssertions;

namespace Cart.Tests.Extensions.Carts;

internal static class CartExtensions
{
    public static Cart.Carts.Cart IsOpenedCartWith(
        this Cart.Carts.Cart cart,
        Guid id,
        Guid clientId)
    {

        cart.Id.Should().Be(id);
        cart.ClientId.Should().Be(clientId);
        cart.Status.Should().Be(CartStatus.Pending);
        cart.Products.Should().BeEmpty();
        cart.TotalPrice.Should().Be(0);

        return cart;
    }

    public static Cart.Carts.Cart HasCartOpenedEventWith(
        this Cart.Carts.Cart cart,
        Guid id,
        Guid clientId)
    {
        var @event = cart.PublishedEvent<CartOpened>();

        @event.Should().NotBeNull();
        @event.Should().BeOfType<CartOpened>();
        @event!.CartId.Should().Be(id);
        @event.ClientId.Should().Be(clientId);

        return cart;
    }
}
