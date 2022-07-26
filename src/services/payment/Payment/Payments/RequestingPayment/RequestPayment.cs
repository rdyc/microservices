using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;
using Payment.Payments.GettingPayments;

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

internal class ValidateRequestPayment : AbstractValidator<RequestPayment>
{
    public ValidateRequestPayment(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<PaymentShortInfo>();
        var collection = database.GetCollection<PaymentShortInfo>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.PaymentId).NotEmpty();

        RuleFor(p => p.OrderId).NotEmpty()
            .MustUniquePaymentForOrder(collection);

        RuleFor(p => p.Amount).GreaterThan(0);
    }
}

internal class HandleRequestPayment : ICommandHandler<RequestPayment>
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