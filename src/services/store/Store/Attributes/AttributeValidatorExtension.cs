using FluentValidation;
using MongoDB.Driver;

namespace Store.Attributes;

public static class AttributeValidatorExtension
{
    public static IRuleBuilderOptions<T, Guid> MustExistAttribute<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<Attribute> collection)
    {
        return ruleBuilder
            .MustAsync(async (value, cancellationToken) =>
                await collection.CountDocumentsAsync(e => e.Id == value, null, cancellationToken) != 0
            )
            .WithMessage("The attribute was not found");
    }
}