using FluentValidation;
using FW.Core.Exceptions;
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
        collection = database.GetCollection<CurrencyShortInfo>("currency_shortinfo");
    }

    protected void ValidateId()
    {
        RuleFor(p => p.Id).NotEmpty();
        Transform(p => p.Id, t => t.Value)
            .MustExistCurrency(collection);
    }

    protected void ValidateName(bool isUpdating = false)
    {
        RuleFor(p => p.Name).NotEmpty()
            .MustUniqueCurrencyName(collection, isUpdating);
    }

    protected void ValidateCode(bool isUpdating = false)
    {
        RuleFor(p => p.Code).NotEmpty().MaximumLength(3)
            .MustUniqueCurrencyCode(collection, isUpdating);
    }

    protected void ValidateSymbol(bool isUpdating = false)
    {
        RuleFor(p => p.Symbol).NotEmpty().MaximumLength(3)
            .MustUniqueCurrencySymbol(collection, isUpdating);
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
            .MustAsync(async (type, value, context, cancellationToken) =>
            {
                var data = await collection.Find(e => e.Id == value)
                    .SingleOrDefaultAsync(cancellationToken);

                if (data == null)
                    throw AggregateNotFoundException.For<T>(value);

                if (!context.RootContextData.ContainsKey(value.ToString()))
                {
                    context.RootContextData[value.ToString()] = data;
                }

                return true;
            });
    }

    public static IRuleBuilderOptions<T, string> MustUniqueCurrencyName<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IMongoCollection<CurrencyShortInfo> collection,
        bool isUpdating = false)
        where T : CurrencyCommand
    {
        return ruleBuilder
            .MustAsync(async (type, value, context, cancellationToken) =>
            {
                var builder = Builders<CurrencyShortInfo>.Filter;
                var filter = builder.Eq(e => e.Name, value);

                if (isUpdating)
                {
                    filter = builder.And(filter, builder.Ne(e => e.Id, type.Id));
                }

                return await collection.CountDocumentsAsync(filter, null, cancellationToken) == 0;
            })
            .WithMessage("The name already used");
    }

    public static IRuleBuilderOptions<T, string> MustUniqueCurrencyCode<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IMongoCollection<CurrencyShortInfo> collection,
        bool isUpdating = false)
        where T : CurrencyCommand
    {
        return ruleBuilder
            .MustAsync(async (type, value, context, cancellationToken) =>
            {
                var builder = Builders<CurrencyShortInfo>.Filter;
                var filter = builder.Eq(e => e.Code, value);

                if (isUpdating)
                {
                    filter = builder.And(filter, builder.Ne(e => e.Id, type.Id));
                }

                return await collection.CountDocumentsAsync(filter, null, cancellationToken) == 0;
            })
            .WithMessage("The code already used");
    }

    public static IRuleBuilderOptions<T, string> MustUniqueCurrencySymbol<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IMongoCollection<CurrencyShortInfo> collection,
        bool isUpdating = false)
        where T : CurrencyCommand
    {
        return ruleBuilder
            .MustAsync(async (type, value, context, cancellationToken) =>
            {
                var builder = Builders<CurrencyShortInfo>.Filter;
                var filter = builder.Eq(e => e.Symbol, value);

                if (isUpdating)
                {
                    filter = builder.And(filter, builder.Ne(e => e.Id, type.Id));
                }

                return await collection.CountDocumentsAsync(filter, null, cancellationToken) == 0;
            })
            .WithMessage("The symbol already used");
    }
}