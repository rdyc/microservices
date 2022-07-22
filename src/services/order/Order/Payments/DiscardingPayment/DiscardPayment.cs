using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Order.Payments.DiscardingPayment;

public class DiscardPayment : ICommand
{
    public Guid PaymentId { get; }
    public DiscardReason DiscardReason { get; }

    private DiscardPayment(Guid paymentId, DiscardReason discardReason)
    {
        PaymentId = paymentId;
        DiscardReason = discardReason;
    }

    public static DiscardPayment Create(Guid paymentId)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));

        return new DiscardPayment(paymentId, DiscardReason.OrderCancelled);
    }
}


public class HandleDiscardPayment : ICommandHandler<DiscardPayment>
{
    private readonly IEventStoreDBRepository<Orders.Order> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleDiscardPayment(
        IEventStoreDBRepository<Orders.Order> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(DiscardPayment command, CancellationToken cancellationToken)
    {
        /* await scope.Do((expectedRevision, eventMetadata) =>
            repository.GetAndUpdate(
                command.PaymentId,
                order => order.DiscardPayment(),
                expectedRevision,
                eventMetadata,
                cancellationToken
            )
        ); */

        return Unit.Value;
    }
}