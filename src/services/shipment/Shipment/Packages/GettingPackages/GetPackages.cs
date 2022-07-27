using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Shipment.Packages.GettingPackages;

public record GetPackages(
    int Index,
    int Size
) : IQuery<IListPaged<PackageShortInfo>>
{
    public static GetPackages Create(int? index = 0, int? size = 10)
    {
        if (index is null or < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (size is null or < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(size));

        return new(index.Value, size.Value);
    }
}

internal class HandleGetPackages : IQueryHandler<GetPackages, IListPaged<PackageShortInfo>>
{
    private readonly IMongoCollection<PackageShortInfo> collection;

    public HandleGetPackages(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<PackageShortInfo>();
        this.collection = database.GetCollection<PackageShortInfo>(collectionName);
    }

    public async Task<IListPaged<PackageShortInfo>> Handle(GetPackages request, CancellationToken cancellationToken)
    {
       var (index, size) = request;

        var filter = Builders<PackageShortInfo>.Filter.Empty;

        return await collection.FindWithPagingAsync(filter, index, size, cancellationToken);
    }
}