using MediatR;
using MongoDB.Driver;

namespace Lookup.Currencies.GettingCurrencies;

public record GetCurrencies(int Index, int Size) : IRequest<IEnumerable<CurrencyShortInfo>>;

internal class HandleGetCurrencies : IRequestHandler<GetCurrencies, IEnumerable<CurrencyShortInfo>>
{
    private readonly IMongoCollection<CurrencyShortInfo> collection;

    public HandleGetCurrencies(IMongoDatabase mongoDb)
    {
        this.collection = mongoDb.GetCollection<CurrencyShortInfo>("currency_shortinfo");
    }

    public async Task<IEnumerable<CurrencyShortInfo>> Handle(GetCurrencies request, CancellationToken cancellationToken)
    {
        var (index, size) = request;

        return await collection.Find(_ => true)
                .Skip(index * (index - 1))
                .Limit(size)
                .ToListAsync(cancellationToken);
    }
}