using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;
using Payment.Payments.GettingPayments;

namespace Payment.Payments.DiscardingPayment;

public record DiscardPayment(
    Guid PaymentId,
    DiscardReason DiscardReason
) : ICommand
{
    public static DiscardPayment Create(Guid? paymentId, DiscardReason? discardReason)
    {
        if (paymentId == null || paymentId == Guid.Empty)
            throw new ArgumentNullException(nameof(paymentId));
        if (discardReason is null or default(DiscardReason))
            throw new InvalidOperationException(nameof(paymentId));

        return new(paymentId.Value, discardReason.Value);
    }
}

internal class ValidateDiscardPayment : AbstractValidator<DiscardPayment>
{
    public ValidateDiscardPayment(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<PaymentShortInfo>();
        var collection = database.GetCollection<PaymentShortInfo>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.PaymentId).NotEmpty()
            .MustMatchPaymentStatus(PaymentStatus.Pending, collection);

        RuleFor(p => p.DiscardReason).NotEmpty();
    }
}

internal class HandleDiscardPayment : ICommandHandler<DiscardPayment>
{
    private readonly IEventStoreDBRepository<Payment> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleDiscardPayment(
        IEventStoreDBRepository<Payment> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(DiscardPayment command, CancellationToken cancellationToken)
    {
        var (paymentId, _) = command;

        await scope.Do((expectedVersion, traceMetadata) =>
            repository.GetAndUpdate(
                paymentId,
                payment => payment.TimeOut(),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}
