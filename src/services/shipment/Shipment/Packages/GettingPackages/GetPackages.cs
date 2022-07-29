using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Shipment.Packages.GettingPackages;

public record GetPackages(
    PagedOption Option
) : IQuery<IListPaged<PackageShortInfo>>
{
    public static GetPackages Create(int? page, int? size) =>
        new(PagedOption.Create(page ?? 1, size ?? 10));
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
        var filter = Builders<PackageShortInfo>.Filter.Empty;

        return await collection.FindWithPagingAsync(filter, request.Option, cancellationToken);
    }
}