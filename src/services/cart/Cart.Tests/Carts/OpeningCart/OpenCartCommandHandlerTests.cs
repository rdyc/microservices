using Cart.Carts.OpeningCart;
using Cart.Tests.Extensions.Carts;
using Cart.Tests.Stubs.Events;
using Cart.Tests.Stubs.Repositories;
using FluentAssertions;
using Xunit;

namespace Cart.Tests.Carts.OpeningCart;

public class OpenCartCommandHandlerTests
{
    [Fact]
    public async Task ForInitCartCommand_ShouldAddNewCart()
    {
        // Given
        var repository = new FakeEventStoreDBRepository<Cart.Carts.Cart>();
        var scope = new FakeEventStoreAppendScope();

        var commandHandler = new HandleOpenCart(
            repository,
            scope
        );

        var command = OpenCart.Create(Guid.NewGuid(), Guid.NewGuid());

        // When
        await commandHandler.Handle(command, CancellationToken.None);

        // Then
        repository.Aggregates.Should().HaveCount(1);

        var cart = repository.Aggregates.Values.Single();

        cart
            .IsOpenedCartWith(
                command.CartId,
                command.ClientId
            )
            .HasCartOpenedEventWith(
                command.CartId,
                command.ClientId
            );
    }
}
