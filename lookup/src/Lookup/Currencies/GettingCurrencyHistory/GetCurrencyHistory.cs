using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using MediatR;
using MongoDB.Driver;

namespace Lookup.Currencies.GettingCurrencyHistory;

public record GetCurrencyHistory(Guid CurrencyId, int Index, int Size) : IRequest<IListPaged<CurrencyHistory>>;

internal class HandleGetCurrencyHistory : IRequestHandler<GetCurrencyHistory, IListPaged<CurrencyHistory>>
{
    private readonly IMongoCollection<CurrencyHistory> collection;

    public HandleGetCurrencyHistory(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<CurrencyHistory>();
        collection = database.GetCollection<CurrencyHistory>(collectionName);
    }

    public async Task<IListPaged<CurrencyHistory>> Handle(GetCurrencyHistory request, CancellationToken cancellationToken)
    {
        var (currencyId, index, size) = request;

        var filter = Builders<CurrencyHistory>.Filter.Eq(e => e.AggregateId, currencyId);

        return await collection.FindWithPagingAsync(filter, index, size, cancellationToken);
    }
}