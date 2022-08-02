using FW.Core.ElasticSearch.Indices;
using FW.Core.Queries;

namespace Search.Products.SearchingProducts;

public record SearchProducts(
    string? Find
) : IQuery<IReadOnlyCollection<Product>>
{
    public static SearchProducts Create(string? find) =>
        new(find?.ToLower());
}

internal class HandleSearchProducts : IQueryHandler<SearchProducts, IReadOnlyCollection<Product>>
{
    private const int MaxItemsCount = 100;

    private readonly Nest.IElasticClient elasticClient;

    public HandleSearchProducts(
        Nest.IElasticClient elasticClient
    )
    {
        this.elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
    }

    public async Task<IReadOnlyCollection<Product>> Handle(SearchProducts query, CancellationToken cancellationToken)
    {
        var response = await elasticClient.SearchAsync<Product>(s => s
                .Index(IndexNameMapper.ToIndexName<Product>())
                .Query(q => q
                    .QueryString(qs => qs.AnalyzeWildcard()
                        .Query($"*{query?.Find}*")
                        .Fields(fs => fs.Fields(
                            f1 => f1.Sku, 
                            f2 => f2.Name, 
                            f3 => f3.Description)
                        )
                    )
                )
                .Query(q => q
                    .Bool(b => b
                        .Should(
                            bs => bs.Term(p => p.IsActive, true)
                        )
                    )
                )
                .Size(MaxItemsCount),
            cancellationToken);

        return response.Documents;
    }
}