using Cart.Products;
using FluentValidation;
using FW.Core.Exceptions;
using MongoDB.Driver;

namespace Cart.Products;

public interface IProduct
{
    Guid ProductId { get; }
}

public static class ProductValidatorExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustExistProduct<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<Product> collection)
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

    public static IRuleBuilderOptions<T, int> MustInStockProduct<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        IMongoCollection<Product> collection
    ) where T : IProduct
    {
        return ruleBuilder
            .MustAsync(async (instance, value, cancellationToken) =>
            {
                var builder = Builders<Product>.Filter;
                var filter = builder.Eq(e => e.Id, instance.ProductId);
                var product = await collection.Find(filter).SingleAsync(cancellationToken);

                return product.Stock >= value;
            })
            .WithMessage("The product is out of stock");
    }
}