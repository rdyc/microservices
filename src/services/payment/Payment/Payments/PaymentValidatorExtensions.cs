using FluentValidation;
using MongoDB.Driver;
using Payment.Payments.GettingPayments;

namespace Payment.Payments;

public static class PaymentValidatorExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustExistPayment<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<PaymentShortInfo> collection
    ) => ruleBuilder
        .MustAsync(async (value, cancellationToken) => await collection
            .CountDocumentsAsync(e => e.Id == value, null, cancellationToken) == 1)
        .WithMessage("The payment was not found");

    public static IRuleBuilderOptions<T, Guid> MustMatchPaymentStatus<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        PaymentStatus status,
        IMongoCollection<PaymentShortInfo> collection
    ) => ruleBuilder
        .MustAsync(async (value, cancellationToken) =>
        {
            var payment = await collection.Find(e => e.Id.Equals(value))
                .SingleAsync(cancellationToken);

            return payment.Status.Equals(status);
        })
        .WithMessage("The payment status was invalid");

    public static IRuleBuilderOptions<T, Guid> MustBeNotExpiredPayment<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<PaymentShortInfo> collection
    ) => ruleBuilder
        .MustAsync(async (value, cancellationToken) =>
        {
            var payment = await collection.Find(e => e.Id.Equals(value))
                .SingleAsync(cancellationToken);

            return payment.ExpiredAt > DateTime.UtcNow;
        })
        .WithMessage("The payment duration was expired");

    public static IRuleBuilderOptions<T, Guid> MustBeExpiredPayment<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<PaymentShortInfo> collection
    ) => ruleBuilder
        .MustAsync(async (value, cancellationToken) =>
        {
            var payment = await collection.Find(e => e.Id.Equals(value))
                .SingleAsync(cancellationToken);

            return payment.ExpiredAt < DateTime.UtcNow;
        })
        .WithMessage("The payment expiration was not ended yet");

    public static IRuleBuilderOptions<T, Guid> MustUniquePaymentForOrder<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<PaymentShortInfo> collection
    )
    {
        return ruleBuilder
            .MustAsync(async (instance, value, cancellationToken) =>
            {
                var builder = Builders<PaymentShortInfo>.Filter;
                var filter = builder.Eq(e => e.OrderId, value);

                return await collection.CountDocumentsAsync(filter, null, cancellationToken) == 0;
            })
            .WithMessage("The payment order already exist");
    }
}