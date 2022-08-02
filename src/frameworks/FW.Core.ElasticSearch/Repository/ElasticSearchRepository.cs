using FW.Core.ElasticSearch.Indices;
using Nest;
using IAggregate = FW.Core.Aggregates.IAggregate;

namespace FW.Core.ElasticSearch.Repository;

public interface IElasticSearchRepository<T> where T : class, IAggregate, new()
{
    Task<T?> Find(Guid id, CancellationToken cancellationToken);
    Task Add(T aggregate, CancellationToken cancellationToken);
    Task Update(T aggregate, CancellationToken cancellationToken);
    Task Delete(T aggregate, CancellationToken cancellationToken);
}

public class ElasticSearchRepository<T> : IElasticSearchRepository<T> where T : class, IAggregate, new()
{
    private readonly IElasticClient elasticClient;

    public ElasticSearchRepository(
        IElasticClient elasticClient
    )
    {
        this.elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
    }

    public async Task<T?> Find(Guid id, CancellationToken cancellationToken)
    {
        var response = await elasticClient.GetAsync(DocumentPath<T>.Id(id).Index(IndexNameMapper.ToIndexName<T>()), ct: cancellationToken);
        return response?.Source;
    }

    public async Task Add(T aggregate, CancellationToken cancellationToken)
    {
        await elasticClient.IndexAsync(aggregate, i => i.Id(aggregate.Id).Index(IndexNameMapper.ToIndexName<T>()), cancellationToken);
    }

    public async Task Update(T aggregate, CancellationToken cancellationToken)
    {
        await elasticClient.UpdateAsync<T>(aggregate.Id, i => i.Doc(aggregate).Index(IndexNameMapper.ToIndexName<T>()), cancellationToken);
    }

    public async Task Delete(T aggregate, CancellationToken cancellationToken)
    {
        await elasticClient.DeleteAsync<T>(aggregate.Id, ct: cancellationToken);
    }
}
