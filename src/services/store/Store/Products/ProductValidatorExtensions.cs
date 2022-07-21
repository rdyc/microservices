using FluentValidation;
using MongoDB.Driver;
using Store.Products.GettingProducts;

namespace Store.Products;

public interface IProduct
{
    Guid Id { get; }
}

public static class ProductValidatorExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustExistProduct<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<ProductShortInfo> collection
    ) => ruleBuilder.MustAsync(async (value, ct) => await collection
            .CountDocumentsAsync(e => e.Id == value, default, ct) > 0)
            .WithMessage("The product was not found");

    public static IRuleBuilderOptions<T, string> MustUniqueProductSku<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IMongoCollection<ProductShortInfo> collection,
        bool isUpdating = false
    ) where T : IProduct
    {
        return ruleBuilder
            .MustAsync(async (instance, value, ct) =>
            {
                var builder = Builders<ProductShortInfo>.Filter;
                var filter = builder.Eq(e => e.Sku, value);

                if (isUpdating)
                {
                    filter = builder.And(filter, builder.Ne(e => e.Id, instance.Id));
                }

                return await collection.CountDocumentsAsync(filter, default, ct) == 0;
            })
            .WithMessage("The product sku already used");
    }
}