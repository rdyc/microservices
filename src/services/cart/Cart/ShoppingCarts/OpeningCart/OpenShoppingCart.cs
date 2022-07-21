using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Cart.ShoppingCarts.OpeningCart;

public record OpenShoppingCart(
    Guid CartId,
    Guid ClientId
) : ICommand
{
    public static OpenShoppingCart Create(Guid? cartId, Guid? clientId)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        if (clientId == null || clientId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(clientId));

        return new(cartId.Value, clientId.Value);
    }
}

internal class ValidateOpenShoppingCart : AbstractValidator<OpenShoppingCart>
{
    public ValidateOpenShoppingCart()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.CartId).NotEmpty();
        RuleFor(p => p.ClientId).NotEmpty();
    }
}

internal class HandleOpenCart : ICommandHandler<OpenShoppingCart>
{
    private readonly IEventStoreDBRepository<ShoppingCart> cartRepository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleOpenCart(
        IEventStoreDBRepository<ShoppingCart> cartRepository,
        IEventStoreDBAppendScope scope
    )
    {
        this.cartRepository = cartRepository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(OpenShoppingCart command, CancellationToken cancellationToken)
    {
        var (cartId, clientId) = command;

        await scope.Do((_, eventMetadata) =>
            cartRepository.Add(
                ShoppingCart.Open(cartId, clientId),
                eventMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}