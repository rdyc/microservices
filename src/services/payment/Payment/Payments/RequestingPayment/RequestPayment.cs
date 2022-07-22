using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Payment.Payments.RequestingPayment;

public record RequestPayment(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount
) : ICommand
{
    public static RequestPayment Create(
        Guid? paymentId,
        Guid? orderId,
        decimal? amount
    )
    {
        if (paymentId == null || paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));
        if (orderId == null || orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (amount is null or <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        return new(paymentId.Value, orderId.Value, amount.Value);
    }
}

public class HandleRequestPayment : ICommandHandler<RequestPayment>
{
    private readonly IEventStoreDBRepository<Payment> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRequestPayment(
        IEventStoreDBRepository<Payment> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RequestPayment command, CancellationToken cancellationToken)
    {
        var (paymentId, orderId, amount) = command;

        await scope.Do((_, eventMetadata) =>
            repository.Add(
                Payment.Initialize(paymentId, orderId, amount),
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}