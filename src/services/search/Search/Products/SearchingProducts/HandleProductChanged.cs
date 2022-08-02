using FW.Core.ElasticSearch.Repository;
using FW.Core.Events;
using Search.Products.ModifyingProduct;
using Search.Products.RegisteringProduct;
using Search.Products.RemovingProduct;

namespace Search.Products.SearchingProducts;

internal class HandleProductChanged :
    IEventHandler<ProductRegistered>,
    IEventHandler<ProductModified>,
    IEventHandler<ProductRemoved>
{
    private readonly IElasticSearchRepository<Product> repository;

    public HandleProductChanged(IElasticSearchRepository<Product> repository)
    {
        this.repository = repository;
    }

    public async Task Handle(ProductRegistered @event, CancellationToken cancellationToken)
    {
        var product = new Product();

        product.When(@event);

        await repository.Add(product, cancellationToken);
    }

    public async Task Handle(ProductModified @event, CancellationToken cancellationToken)
    {
        var product = await repository.Find(@event.Id, cancellationToken);

        if (product != null)
        {
            product.When(@event);

            await repository.Update(product, cancellationToken);
        }
    }

    public async Task Handle(ProductRemoved @event, CancellationToken cancellationToken)
    {
        var product = await repository.Find(@event.Id, cancellationToken);

        if (product != null)
        {
            product.When(@event);

            await repository.Delete(product, cancellationToken);
        }
    }
}