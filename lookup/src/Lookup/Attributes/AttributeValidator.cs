using FluentValidation;
using FW.Core.Exceptions;
using FW.Core.MongoDB;
using Lookup.Attributes.GettingAttributes;
using MongoDB.Driver;

namespace Lookup.Attributes;

public class AttributeValidator<T> : AbstractValidator<T>
    where T : AttributeCommand
{
    private readonly IMongoCollection<AttributeShortInfo> collection;

    public AttributeValidator(IMongoDatabase database)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        var collectionName = MongoHelper.GetCollectionName<AttributeShortInfo>();
        collection = database.GetCollection<AttributeShortInfo>(collectionName);
    }

    protected void ValidateId()
    {
        RuleFor(p => p.Id).NotEmpty();
        Transform(p => p.Id, t => t.Value)
            .MustExistAttribute(collection);
    }

    protected void ValidateName()
    {
        RuleFor(p => p.Name).NotEmpty();;
    }

    protected void ValidateUnit(bool isUpdating = false)
    {
        RuleFor(p => p.Unit).NotEmpty().MaximumLength(3)
            .MustUniqueAttributeUnit(collection, isUpdating);
    }
}

public static class ValidatorExtension
{
    public static IRuleBuilderOptions<T, Guid> MustExistAttribute<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<AttributeShortInfo> collection)
        where T : AttributeCommand
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

    public static IRuleBuilderOptions<T, string> MustUniqueAttributeUnit<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IMongoCollection<AttributeShortInfo> collection,
        bool isUpdating = false)
        where T : AttributeCommand
    {
        return ruleBuilder
            .MustAsync(async (instance, value, cancellationToken) =>
            {
                var builder = Builders<AttributeShortInfo>.Filter;
                var filter = builder.Eq(e => e.Unit, value);

                if (isUpdating)
                {
                    filter = builder.And(filter, builder.Ne(e => e.Id, instance.Id));
                }

                return await collection.CountDocumentsAsync(filter, null, cancellationToken) == 0;
            })
            .WithMessage("The attribute unit already used");
    }
}