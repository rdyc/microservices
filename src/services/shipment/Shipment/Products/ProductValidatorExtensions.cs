using FluentValidation;
using MongoDB.Driver;
using Shipment.Packages.SendingPackage;

namespace Shipment.Products;

public static class ProductValidatorExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustExistProduct<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<Product> collection
    ) => ruleBuilder
        .MustAsync(async (value, cancellationToken) => await collection
            .CountDocumentsAsync(e => e.Id == value, null, cancellationToken) == 1)
        .WithMessage("The product was not found");

    public static IRuleBuilderOptions<T, int> MustInStockProduct<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        IMongoCollection<Product> collection
    ) where T : PackageItem => ruleBuilder
        .MustAsync(async (instance, value, cancellationToken) =>
        {
            var product = await collection.Find(e => e.Id.Equals(instance.ProductId))
                .SingleAsync(cancellationToken);

            return product.Stock >= value;
        })
        .WithMessage("The product is out of stock");
}