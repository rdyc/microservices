using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Cart.Carts.OpeningCart;

public record OpenCart(
    Guid CartId,
    Guid ClientId
) : ICommand
{
    public static OpenCart Create(Guid? cartId, Guid? clientId)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        if (clientId == null || clientId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(clientId));

        return new(cartId.Value, clientId.Value);
    }
}

internal class ValidateOpenCart : AbstractValidator<OpenCart>
{
    public ValidateOpenCart()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.CartId).NotEmpty();
        RuleFor(p => p.ClientId).NotEmpty();
    }
}

internal class HandleOpenCart : ICommandHandler<OpenCart>
{
    private readonly IEventStoreDBRepository<Cart> cartRepository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleOpenCart(
        IEventStoreDBRepository<Cart> cartRepository,
        IEventStoreDBAppendScope scope
    )
    {
        this.cartRepository = cartRepository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(OpenCart request, CancellationToken cancellationToken)
    {
        var (cartId, clientId) = request;

        await scope.Do((_, eventMetadata) =>
            cartRepository.Add(
                Cart.Open(cartId, clientId),
                eventMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}