using FluentValidation;
using FW.Core.Exceptions;
using MongoDB.Driver;
using Store.Products.GettingProducts;

namespace Store.Products;

public interface IProduct
{
    Guid ProductId { get; }
}

public static class ProductValidatorExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustExistProduct<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<ProductShortInfo> collection)
    {
        return ruleBuilder
            .MustAsync(async (value, cancellationToken) =>
            {
                var count = await collection.CountDocumentsAsync(e => e.Id == value, null, cancellationToken);

                if (count == 0)
                    throw AggregateNotFoundException.For<T>(value);

                return true;
            });
    }

    public static IRuleBuilderOptions<T, string> MustUniqueProductSKU<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IMongoCollection<ProductShortInfo> collection,
        bool isUpdating = false
    ) where T : IProduct
    {
        return ruleBuilder
            .MustAsync(async (instance, value, cancellationToken) =>
            {
                var builder = Builders<ProductShortInfo>.Filter;
                var filter = builder.Eq(e => e.Sku, value);

                if (isUpdating)
                {
                    filter = builder.And(filter, builder.Ne(e => e.Id, instance.ProductId));
                }

                return await collection.CountDocumentsAsync(filter, null, cancellationToken) == 0;
            })
            .WithMessage("The product sku already used");
    }
}