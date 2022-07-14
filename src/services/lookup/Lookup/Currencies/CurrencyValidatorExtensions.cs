using FluentValidation;
using FW.Core.Exceptions;
using Lookup.Currencies.GettingCurrencies;
using MongoDB.Driver;

namespace Lookup.Currencies;

public interface ICurrency
{
    Guid CurrencyId { get; }
}

public static class CurrencyValidatorExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustExistCurrency<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<CurrencyShortInfo> collection)
    {
        return ruleBuilder
            .MustAsync(async (value, cancellationToken) =>
            {
                var count = await collection.CountDocumentsAsync(
                    e => e.Id.Equals(value) && !e.Status.Equals(LookupStatus.Removed),
                    null,
                    cancellationToken);

                if (count == 0)
                    throw AggregateNotFoundException.For<T>(value);

                return true;
            });
    }

    public static IRuleBuilderOptions<T, string> MustUniqueCurrencyCode<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IMongoCollection<CurrencyShortInfo> collection,
        bool isUpdating = false)
        where T : ICurrency
    {
        return ruleBuilder
            .MustAsync(async (instance, value, cancellationToken) =>
            {
                var builder = Builders<CurrencyShortInfo>.Filter;
                var filter = builder.Eq(e => e.Code, value);

                if (isUpdating)
                {
                    filter = builder.And(filter, builder.Ne(e => e.Id, instance.CurrencyId));
                }

                return await collection.CountDocumentsAsync(
                    filter,
                    null,
                    cancellationToken) == 0;
            })
            .WithMessage("The currency code already used");
    }
}