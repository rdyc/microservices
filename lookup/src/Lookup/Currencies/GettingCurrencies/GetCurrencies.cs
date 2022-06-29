using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using MediatR;
using MongoDB.Driver;

namespace Lookup.Currencies.GettingCurrencies;

public record GetCurrencies(int Index, int Size) : IRequest<IListPaged<CurrencyShortInfo>>;

internal class HandleGetCurrencies : IRequestHandler<GetCurrencies, IListPaged<CurrencyShortInfo>>
{
    private readonly IMongoCollection<CurrencyShortInfo> collection;

    public HandleGetCurrencies(IMongoDatabase mongoDb)
    {
        this.collection = mongoDb.GetCollection<CurrencyShortInfo>("currency_shortinfo");
    }

    public async Task<IListPaged<CurrencyShortInfo>> Handle(GetCurrencies request, CancellationToken cancellationToken)
    {
        var (index, size) = request;

        var filter = Builders<CurrencyShortInfo>.Filter.Empty;

        return await collection.FindWithPagingAsync(filter, index, size, cancellationToken);
    }
}