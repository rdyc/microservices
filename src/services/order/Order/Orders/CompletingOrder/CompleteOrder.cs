using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Order.Orders.CompletingOrder;

public record CompleteOrder(
    Guid OrderId,
    DateTime CompletedAt
) : ICommand
{
    public static CompleteOrder Create(Guid? orderId, DateTime completedAt)
    {
        if (orderId == null || orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));

        return new CompleteOrder(orderId.Value, completedAt);
    }
}

public class HandleCompleteOrder : ICommandHandler<CompleteOrder>
{
    private readonly IEventStoreDBRepository<Order> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleCompleteOrder(
        IEventStoreDBRepository<Order> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(CompleteOrder request, CancellationToken cancellationToken)
    {
        var (orderId, completedAt) = request;

        await scope.Do((expectedVersion, traceMetadata) =>
            repository.GetAndUpdate(
                orderId,
                order => order.Complete(completedAt),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}