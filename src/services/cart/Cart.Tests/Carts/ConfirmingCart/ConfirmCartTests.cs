using Cart.Carts;
using Cart.Carts.ConfirmingCart;
using Cart.Tests.Builders;
using FW.Core.Testing;
using FluentAssertions;
using Xunit;

namespace Cart.Tests.Carts.ConfirmingCart;

public class ConfirmCartTests
{
    [Fact]
    public void ForTentativeCart_ShouldSucceed()
    {
        // Given
        var cart = CartBuilder
            .Create()
            .Opened()
            .Build();

        // When
        cart.Confirm();

        // Then
        cart.Status.Should().Be(CartStatus.Confirmed);

        var @event = cart.PublishedEvent<CartConfirmed>();

        @event.Should().NotBeNull();
        @event.Should().BeOfType<CartConfirmed>();
        @event!.Id.Should().Be(cart.Id);
    }
}
