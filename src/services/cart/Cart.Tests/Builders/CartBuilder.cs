using FW.Core.Aggregates;

namespace Cart.Tests.Builders;

internal class CartBuilder
{
    private Func<Cart.Carts.Cart> build  = () => new Cart.Carts.Cart();

    public CartBuilder Opened()
    {
        var cartId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        // When
        var cart = Cart.Carts.Cart.Open(
            cartId,
            clientId
        );

        build = () => cart;

        return this;
    }

    public static CartBuilder Create() => new();

    public Cart.Carts.Cart Build()
    {
        var cart = build();
        ((IAggregate)cart).DequeueUncommittedEvents();
        return cart;
    }
}
