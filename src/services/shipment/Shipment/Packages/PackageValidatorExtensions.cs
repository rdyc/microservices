using FluentValidation;
using MongoDB.Driver;
using Shipment.Packages.GettingPackages;
using Shipment.Packages.PreparingPackage;

namespace Shipment.Packages;

public static class PackageValidatorExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustExistPackage<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<PackageShortInfo> collection
    ) => ruleBuilder.MustAsync(async (value, ct) => await collection
            .CountDocumentsAsync(e => e.Id == value, default, ct) > 0)
            .WithMessage("The package was not found");

    public static IRuleBuilderOptions<T, Guid> MustUniquePackageForOrder<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<PackageShortInfo> collection
    ) where T : PreparePackage
    {
        return ruleBuilder
            .MustAsync(async (instance, value, ct) =>
            {
                var builder = Builders<PackageShortInfo>.Filter;
                var filter = builder.Eq(e => e.OrderId, value);

                return await collection.CountDocumentsAsync(filter, default, ct) == 0;
            })
            .WithMessage("The package for orders already exist");
    }
}