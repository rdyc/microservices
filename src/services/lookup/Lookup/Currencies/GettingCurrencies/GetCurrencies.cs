using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Lookup.Currencies.GettingCurrencies;

public record GetCurrencies(
    int Index,
    int Size
) : IQuery<IListPaged<CurrencyShortInfo>>
{
    public static GetCurrencies Create(int index, int size) =>
        new(index, size);
}

internal class HandleGetCurrencies : IQueryHandler<GetCurrencies, IListPaged<CurrencyShortInfo>>
{
    private readonly IMongoCollection<CurrencyShortInfo> collection;

    public HandleGetCurrencies(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<CurrencyShortInfo>();
        collection = database.GetCollection<CurrencyShortInfo>(collectionName);
    }

    public async Task<IListPaged<CurrencyShortInfo>> Handle(GetCurrencies request, CancellationToken cancellationToken)
    {
        var (index, size) = request;

        var filter = Builders<CurrencyShortInfo>.Filter.Empty;

        return await collection.FindWithPagingAsync(filter, index, size, cancellationToken);
    }
}