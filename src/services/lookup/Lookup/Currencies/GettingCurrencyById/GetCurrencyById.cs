using FW.Core.Exceptions;
using FW.Core.MongoDB;
using FW.Core.Queries;
using Lookup.Currencies.GettingCurrencies;
using MongoDB.Driver;

namespace Lookup.Currencies.GettingCurrencyById;

public record GetCurrencyById(Guid Id) : IQuery<CurrencyShortInfo>;

internal class HandleGetCurrencyById : IQueryHandler<GetCurrencyById, CurrencyShortInfo>
{
    private readonly IMongoCollection<CurrencyShortInfo> collection;

    public HandleGetCurrencyById(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<CurrencyShortInfo>();
        collection = database.GetCollection<CurrencyShortInfo>(collectionName);
    }

    public async Task<CurrencyShortInfo> Handle(GetCurrencyById request, CancellationToken cancellationToken)
    {
        var filter = Builders<CurrencyShortInfo>.Filter.Eq(e => e.Id, request.Id);

        var result = await collection.Find(filter).SingleOrDefaultAsync(cancellationToken);

        if (result == null)
            throw AggregateNotFoundException.For<CurrencyShortInfo>(request.Id);

        return result;
    }
}