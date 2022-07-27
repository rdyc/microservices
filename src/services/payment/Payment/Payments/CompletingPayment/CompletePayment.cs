using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;
using Payment.Payments.DiscardingPayment;
using Payment.Payments.GettingPayments;

namespace Payment.Payments.CompletingPayment;

public record CompletePayment(
    Guid PaymentId,
    DateTime CompletedAt
) : ICommand
{
    public static CompletePayment Create(Guid? paymentId, DateTime completedAt)
    {
        if (paymentId == null || paymentId == Guid.Empty)
            throw new ArgumentNullException(nameof(paymentId));

        return new(paymentId.Value, completedAt);
    }
}

internal class ValidateCompletePayment : AbstractValidator<CompletePayment>
{
    public ValidateCompletePayment(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<PaymentShortInfo>();
        var collection = database.GetCollection<PaymentShortInfo>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.PaymentId).NotEmpty()
            .MustExistPayment(collection)
            .MustBeNotExpiredPayment(collection);
    }
}


internal class HandleCompletePayment : ICommandHandler<CompletePayment>
{
    private readonly IEventStoreDBRepository<Payment> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleCompletePayment(
        IEventStoreDBRepository<Payment> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(CompletePayment request, CancellationToken cancellationToken)
    {
        var (paymentId, completedAt) = request;

        await scope.Do(async (expectedVersion, traceMetadata) =>
            {
                try
                {
                    return await repository.GetAndUpdate(
                        paymentId,
                        payment => payment.Complete(completedAt),
                        expectedVersion,
                        traceMetadata,
                        cancellationToken
                    );
                }
                catch
                {
                    return await repository.GetAndUpdate(
                        paymentId,
                        payment => payment.Discard(DiscardReason.UnexpectedError, DateTime.UtcNow),
                        expectedVersion,
                        traceMetadata,
                        cancellationToken
                    );
                }
            }
        );

        return Unit.Value;
    }
}