using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Order.Orders.CompletingOrder;

public record CompleteOrder(
    Guid OrderId
) : ICommand
{
    public static CompleteOrder Create(Guid? orderId)
    {
        if (orderId == null || orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));

        return new CompleteOrder(orderId.Value);
    }
}

public class HandleCompleteOrder : ICommandHandler<CompleteOrder>
{
    private readonly IEventStoreDBRepository<Order> orderRepository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleCompleteOrder(
        IEventStoreDBRepository<Order> orderRepository,
        IEventStoreDBAppendScope scope
    )
    {
        this.orderRepository = orderRepository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(CompleteOrder command, CancellationToken cancellationToken)
    {
        await scope.Do((expectedVersion, traceMetadata) =>
            orderRepository.GetAndUpdate(
                command.OrderId,
                order => order.Complete(),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}