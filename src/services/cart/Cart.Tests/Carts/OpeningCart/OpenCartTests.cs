using Cart.Tests.Extensions.Carts;
using Xunit;

namespace Cart.Tests.Carts.OpeningCart;

public class OpenCartTests
{
    [Fact]
    public void ForValidParams_ShouldCreateCartWithPendingStatus()
    {
        // Given
        var cartId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        // When
        var cart = Cart.Carts.Cart.Open(
            cartId,
            clientId
        );

        // Then
        cart
            .IsOpenedCartWith(
                cartId,
                clientId
            )
            .HasCartOpenedEventWith(
                cartId,
                clientId
            );
    }
}