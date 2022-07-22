using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Cart.ShoppingCarts.CancelingCart;

public record CancelShoppingCart(
    Guid CartId
) : ICommand
{
    public static CancelShoppingCart Create(Guid? cartId)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        return new(cartId.Value);
    }
}

internal class ValidateCancelShoppingCart : AbstractValidator<CancelShoppingCart>
{
    public ValidateCancelShoppingCart()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.CartId).NotEmpty();
    }
}

internal class HandleCancelCart : ICommandHandler<CancelShoppingCart>
{
    private readonly IEventStoreDBRepository<ShoppingCart> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleCancelCart(
        IEventStoreDBRepository<ShoppingCart> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(CancelShoppingCart command, CancellationToken cancellationToken)
    {
        await scope.Do((expectedRevision, eventMetadata) =>
            repository.GetAndUpdate(
                command.CartId,
                cart => cart.Cancel(),
                expectedRevision,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}