using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Store.Products.UpdatingStock;

public record UpdateProductStock(
    Guid ProductId,
    int Stock
) : ICommand
{
    public static UpdateProductStock Create(Guid productId, int stock)
    {
        if (productId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(productId));

        if (stock < 0)
            throw new InvalidOperationException(nameof(stock));

        return new(productId, stock);
    }
}

internal class ValidateUpdateProductStock : AbstractValidator<UpdateProductStock>
{
    public ValidateUpdateProductStock()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.Stock).GreaterThanOrEqualTo(0);
    }
}

internal class HandleUpdateProductStock : ICommandHandler<UpdateProductStock>
{
    private readonly IEventStoreDBRepository<Product> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleUpdateProductStock(IEventStoreDBRepository<Product> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(UpdateProductStock request, CancellationToken cancellationToken)
    {
        var (id, stock) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                id,
                (product) => product.UpdateStock(stock),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}