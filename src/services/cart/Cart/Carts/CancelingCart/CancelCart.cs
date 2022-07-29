using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Cart.Carts.CancelingCart;

public record CancelCart(
    Guid CartId
) : ICommand
{
    public static CancelCart Create(Guid? cartId)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        return new(cartId.Value);
    }
}

internal class ValidateCancelCart : AbstractValidator<CancelCart>
{
    public ValidateCancelCart()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.CartId).NotEmpty();
    }
}

internal class HandleCancelCart : ICommandHandler<CancelCart>
{
    private readonly IEventStoreDBRepository<Cart> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleCancelCart(
        IEventStoreDBRepository<Cart> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(CancelCart command, CancellationToken cancellationToken)
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