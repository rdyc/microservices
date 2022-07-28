using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Order.Orders.CancellingOrder;

public record CancelOrder(
    Guid OrderId,
    OrderCancellationReason CancellationReason,
    DateTime CancelledAt
) : ICommand
{
    public static CancelOrder Create(Guid? orderId, OrderCancellationReason? cancellationReason, DateTime cancelledAt)
    {
        if (!orderId.HasValue)
            throw new ArgumentNullException(nameof(orderId));

        if (!cancellationReason.HasValue)
            throw new ArgumentNullException(nameof(cancellationReason));

        return new CancelOrder(orderId.Value, cancellationReason.Value, cancelledAt);
    }
}

public class HandleCancelOrder : ICommandHandler<CancelOrder>
{
    private readonly IEventStoreDBRepository<Order> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleCancelOrder(
        IEventStoreDBRepository<Order> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(CancelOrder request, CancellationToken cancellationToken)
    {
        var (orderId, reason, cancelledAt) = request;

        await scope.Do((expectedVersion, traceMetadata) =>
            repository.GetAndUpdate(
                orderId,
                (order) => order.Cancel(reason, cancelledAt),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}