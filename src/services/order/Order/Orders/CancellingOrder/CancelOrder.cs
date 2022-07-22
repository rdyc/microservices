using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Order.Orders.CancellingOrder;

public record CancelOrder(
    Guid OrderId,
    OrderCancellationReason CancellationReason
) : ICommand
{
    public static CancelOrder Create(Guid? orderId, OrderCancellationReason? cancellationReason)
    {
        if (!orderId.HasValue)
            throw new ArgumentNullException(nameof(orderId));

        if (!cancellationReason.HasValue)
            throw new ArgumentNullException(nameof(cancellationReason));

        return new CancelOrder(orderId.Value, cancellationReason.Value);
    }
}

public class HandleCancelOrder : ICommandHandler<CancelOrder>
{
    private readonly IEventStoreDBRepository<Order> orderRepository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleCancelOrder(
        IEventStoreDBRepository<Order> orderRepository,
        IEventStoreDBAppendScope scope
    )
    {
        this.orderRepository = orderRepository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(CancelOrder command, CancellationToken cancellationToken)
    {
        await scope.Do((expectedVersion, traceMetadata) =>
            orderRepository.GetAndUpdate(
                command.OrderId,
                order => order.Cancel(command.CancellationReason),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}