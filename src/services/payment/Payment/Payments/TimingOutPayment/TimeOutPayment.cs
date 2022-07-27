using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;
using Payment.Payments.GettingPayments;

namespace Payment.Payments.TimingOutPayment;

public record TimeOutPayment(
    Guid PaymentId,
    DateTime TimedOutAt
) : ICommand
{
    public static TimeOutPayment Create(Guid? paymentId, DateTime? timedOutAt)
    {
        if (paymentId == null || paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));
        if (timedOutAt == null || timedOutAt == default(DateTime))
            throw new ArgumentOutOfRangeException(nameof(timedOutAt));

        return new(paymentId.Value, timedOutAt.Value);
    }
}

internal class ValidateTimeOutPayment : AbstractValidator<TimeOutPayment>
{
    public ValidateTimeOutPayment(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<PaymentShortInfo>();
        var collection = database.GetCollection<PaymentShortInfo>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.PaymentId).NotEmpty()
            .MustBeExpiredPayment(collection)
            .MustMatchPaymentStatus(PaymentStatus.Pending, collection);

        RuleFor(p => p.TimedOutAt).NotEmpty();
    }
}

internal class HandleTimeOutPayment : ICommandHandler<TimeOutPayment>
{
    private readonly IEventStoreDBRepository<Payment> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleTimeOutPayment(
        IEventStoreDBRepository<Payment> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(TimeOutPayment command, CancellationToken cancellationToken)
    {
        var (paymentId, timedOutAt) = command;

        await scope.Do((expectedVersion, traceMetadata) =>
            repository.GetAndUpdate(
                paymentId,
                payment => payment.TimeOut(timedOutAt),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}