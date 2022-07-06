using FW.Core.MongoDB;
using FW.Core.Pagination;
using FW.Core.Queries;
using Lookup.Currencies.GettingCurrencies;
using MongoDB.Driver;

namespace Lookup.Currencies.GettingCurrencyHistory;

public record GetCurrencyList(CurrencyStatus? Status) : IQuery<IListUnpaged<CurrencyShortInfo>>;

internal class HandleGetCurrencyList : IQueryHandler<GetCurrencyList, IListUnpaged<CurrencyShortInfo>>
{
    private readonly IMongoCollection<CurrencyShortInfo> collection;

    public HandleGetCurrencyList(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<CurrencyShortInfo>();
        collection = database.GetCollection<CurrencyShortInfo>(collectionName);
    }

    public async Task<IListUnpaged<CurrencyShortInfo>> Handle(GetCurrencyList request, CancellationToken cancellationToken)
    {
        var filter = Builders<CurrencyShortInfo>.Filter.Eq(e => e.Status, request.Status.HasValue ? request.Status : CurrencyStatus.Active);

        var data = await collection.Find(filter).ToListAsync(cancellationToken);

        return ListUnpaged<CurrencyShortInfo>.Create(data);
    }
}