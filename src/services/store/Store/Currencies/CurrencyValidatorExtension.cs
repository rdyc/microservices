using FluentValidation;
using MongoDB.Driver;

namespace Store.Currencies;

public static class CurrencyValidatorExtension
{
    public static IRuleBuilderOptions<T, Guid> MustExistCurrency<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<Currency> collection)
    {
        return ruleBuilder
            .MustAsync(async (value, cancellationToken) =>
                await collection.CountDocumentsAsync(e => e.Id == value, null, cancellationToken) != 0
            )
            .WithMessage("The currency was not found");
    }
}