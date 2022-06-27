using MongoDB.Driver;

namespace Lookup.Currencies.GettingCurrencies;

public record GetCurrencies(int Index, int Size)
{
    public static async Task<IReadOnlyList<CurrencyShortInfo>> Handle(
        IMongoCollection<CurrencyShortInfo> currencies,
        GetCurrencies query,
        CancellationToken ct
    )
    {
        // var collection = mongoClient.GetDatabase("lookup_currency").GetCollection<CurrencyShortInfo>("currency_shortinfo");
        var (index, size) = query;
        return await currencies.Find(_ => true)
            .Skip(index * (index - 1))
            .Limit(size)
            .ToListAsync(ct);
    }
}