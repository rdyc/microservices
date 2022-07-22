using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Order.Orders.RecordingOrderPayment;

public record RecordOrderPayment(
    Guid OrderId,
    Guid PaymentId,
    DateTime RecordedAt
) : ICommand
{
    public static RecordOrderPayment Create(Guid? orderId, Guid? paymentId, DateTime? RecordedAt)
    {
        if (orderId == null || orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (paymentId == null || paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));
        if (RecordedAt == null || RecordedAt == default(DateTime))
            throw new ArgumentOutOfRangeException(nameof(RecordedAt));

        return new(orderId.Value, paymentId.Value, RecordedAt.Value);
    }
}

public class HandleRecordOrderPayment : ICommandHandler<RecordOrderPayment>
{
    private readonly IEventStoreDBRepository<Order> orderRepository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRecordOrderPayment(
        IEventStoreDBRepository<Order> orderRepository,
        IEventStoreDBAppendScope scope
    )
    {
        this.orderRepository = orderRepository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RecordOrderPayment command, CancellationToken cancellationToken)
    {
        var (orderId, paymentId, recordedAt) = command;

        await scope.Do((expectedVersion, traceMetadata) =>
            orderRepository.GetAndUpdate(
                orderId,
                order => order.RecordPayment(paymentId, recordedAt),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}