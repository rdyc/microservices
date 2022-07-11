using FluentValidation;
using FW.Core.Exceptions;
using FW.Core.MongoDB;
using Lookup.Currencies.GettingCurrencies;
using MongoDB.Driver;

namespace Lookup.Currencies;

public class CurrencyValidator<T> : AbstractValidator<T>
    where T : CurrencyCommand
{
    private readonly IMongoCollection<CurrencyShortInfo> collection;

    public CurrencyValidator(IMongoDatabase database)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        var collectionName = MongoHelper.GetCollectionName<CurrencyShortInfo>();
        collection = database.GetCollection<CurrencyShortInfo>(collectionName);
    }

    protected void ValidateId()
    {
        RuleFor(p => p.Id).NotEmpty();
        Transform(p => p.Id, t => t.Value)
            .MustExistCurrency(collection);
    }

    protected void ValidateName()
    {
        RuleFor(p => p.Name).NotEmpty();;
    }

    protected void ValidateCode(bool isUpdating = false)
    {
        RuleFor(p => p.Code).NotEmpty().MaximumLength(3)
            .MustUniqueCurrencyCode(collection, isUpdating);
    }

    protected void ValidateSymbol()
    {
        RuleFor(p => p.Symbol).NotEmpty().MaximumLength(3);
    }
}

public static class ValidatorExtension
{
    public static IRuleBuilderOptions<T, Guid> MustExistCurrency<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<CurrencyShortInfo> collection)
        where T : CurrencyCommand
    {
        return ruleBuilder
            .MustAsync(async (value, cancellationToken) =>
            {
                var count = await collection.CountDocumentsAsync(e => e.Id == value && e.Status != LookupStatus.Removed, null, cancellationToken);

                if (count == 0)
                    throw AggregateNotFoundException.For<T>(value);

                return true;
            });
    }

    public static IRuleBuilderOptions<T, string> MustUniqueCurrencyCode<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IMongoCollection<CurrencyShortInfo> collection,
        bool isUpdating = false)
        where T : CurrencyCommand
    {
        return ruleBuilder
            .MustAsync(async (instance, value, cancellationToken) =>
            {
                var builder = Builders<CurrencyShortInfo>.Filter;
                var filter = builder.Eq(e => e.Code, value);

                if (isUpdating)
                {
                    filter = builder.And(filter, builder.Ne(e => e.Id, instance.Id));
                }

                return await collection.CountDocumentsAsync(filter, null, cancellationToken) == 0;
            })
            .WithMessage("The currency code already used");
    }
}