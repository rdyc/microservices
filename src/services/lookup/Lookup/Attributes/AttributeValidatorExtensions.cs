using FluentValidation;
using FW.Core.Exceptions;
using Lookup.Attributes.GettingAttributes;
using MongoDB.Driver;

namespace Lookup.Attributes;

public interface IAttribute
{
    Guid AttributeId { get; }
}

public static class AttributeValidatorExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustExistAttribute<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<AttributeShortInfo> collection
    )
    {
        return ruleBuilder
            .MustAsync(async (value, cancellationToken) =>
            {
                var count = await collection.CountDocumentsAsync(
                    e => e.Id.Equals(value) && !e.Status.Equals(LookupStatus.Removed),
                    null,
                    cancellationToken);

                if (count == 0)
                    throw AggregateNotFoundException.For<AttributeShortInfo>(value);

                return true;
            });
    }

    public static IRuleBuilderOptions<T, string> MustUniqueAttributeUnit<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IMongoCollection<AttributeShortInfo> collection,
        bool isUpdating = false
    ) where T : IAttribute
    {
        return ruleBuilder
            .MustAsync(async (instance, value, cancellationToken) =>
            {
                var builder = Builders<AttributeShortInfo>.Filter;
                var filter = builder.Eq(e => e.Unit, value);

                if (isUpdating)
                {
                    filter = builder.And(filter, builder.Ne(e => e.Id, instance.AttributeId));
                }

                return await collection.CountDocumentsAsync(filter, null, cancellationToken) == 0;
            })
            .WithMessage("The attribute unit already used");
    }
}