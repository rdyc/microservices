using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Order.Orders.ProcessingOrder;

public record ProcessOrder(
    Guid OrderId,
    Guid PackageId,
    DateTime ProcessedAt
) : ICommand
{
    public static ProcessOrder Create(Guid? orderId, Guid? packageId, DateTime processedAt)
    {
        if (orderId == null || orderId == Guid.Empty)
            throw new ArgumentNullException(nameof(orderId));

        if (packageId == null || packageId == Guid.Empty)
            throw new ArgumentNullException(nameof(orderId));

        return new ProcessOrder(orderId.Value, packageId.Value, processedAt);
    }
}

public class HandleProcessOrder : ICommandHandler<ProcessOrder>
{
    private readonly IEventStoreDBRepository<Order> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleProcessOrder(
        IEventStoreDBRepository<Order> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(ProcessOrder request, CancellationToken cancellationToken)
    {
        var (orderId, packageId, processedAt) = request;

        await scope.Do((expectedVersion, traceMetadata) =>
            repository.GetAndUpdate(
                orderId,
                order => order.Process(packageId, processedAt),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}