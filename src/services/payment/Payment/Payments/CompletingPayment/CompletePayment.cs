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
    Guid PaymentId
) : ICommand
{
    public static CompletePayment Create(Guid? paymentId)
    {
        if (paymentId == null || paymentId == Guid.Empty)
            throw new ArgumentNullException(nameof(paymentId));

        return new(paymentId.Value);
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

    public async Task<Unit> Handle(CompletePayment command, CancellationToken cancellationToken)
    {
        var paymentId = command.PaymentId;

        await scope.Do(async (expectedVersion, traceMetadata) =>
            {
                try
                {
                    return await repository.GetAndUpdate(
                        paymentId,
                        payment => payment.Complete(),
                        expectedVersion,
                        traceMetadata,
                        cancellationToken
                    );
                }
                catch
                {
                    return await repository.GetAndUpdate(
                        paymentId,
                        payment => payment.Discard(DiscardReason.UnexpectedError),
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