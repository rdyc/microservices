using FW.Core.Exceptions;
using FW.Core.MongoDB;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Shipment.Packages.GettingPackageById;

public record GetPackageById(
    Guid PackageId
) : IQuery<PackageDetails>
{
    public static GetPackageById Create(Guid packageId) =>
        new(packageId);
}

internal class HandleGetPackageById : IQueryHandler<GetPackageById, PackageDetails>
{
    private readonly IMongoCollection<PackageDetails> collection;

    public HandleGetPackageById(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<PackageDetails>();
        this.collection = database.GetCollection<PackageDetails>(collectionName);
    }

    public async Task<PackageDetails> Handle(GetPackageById request, CancellationToken cancellationToken)
    {
        var result = await collection
            .Find(e => e.Id.Equals(request.PackageId))
            .SingleOrDefaultAsync(cancellationToken);

        if (result is null)
            throw AggregateNotFoundException.For<PackageDetails>(request.PackageId);

        return result;
    }
}