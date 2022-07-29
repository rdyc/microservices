using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Cart.Carts.ConfirmingCart;

public record ConfirmCart(
    Guid CartId
) : ICommand
{
    public static ConfirmCart Create(Guid? cartId)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        return new ConfirmCart(cartId.Value);
    }
}

internal class ValidateConfirmCart : AbstractValidator<ConfirmCart>
{
    public ValidateConfirmCart()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.CartId).NotEmpty();
    }
}

internal class HandleConfirmCart : ICommandHandler<ConfirmCart>
{
    private readonly IEventStoreDBRepository<Cart> cartRepository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleConfirmCart(
        IEventStoreDBRepository<Cart> cartRepository,
        IEventStoreDBAppendScope scope
    )
    {
        this.cartRepository = cartRepository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(ConfirmCart command, CancellationToken cancellationToken)
    {
        await scope.Do((expectedRevision, eventMetadata) =>
            cartRepository.GetAndUpdate(
                command.CartId,
                cart => cart.Confirm(),
                expectedRevision,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}