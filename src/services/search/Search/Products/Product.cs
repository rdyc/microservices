using FW.Core.Aggregates;
using FW.Core.ElasticSearch.Repository;
using FW.Core.Events;
using Newtonsoft.Json;
using Search.Products.ModifyingProduct;
using Search.Products.RegisteringProduct;
using Search.Products.RemovingProduct;

namespace Search.Products;

public class Product : Aggregate
{
    public string Sku { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public ProductStatus Status { get; private set; }

    public Product() { }

    [JsonConstructor]
    public Product(Guid id, string sku, string name, string description)
    {
        Id = id;
        Sku = sku;
        Name = name;
        Description = description;
    }

    public void Apply(ProductRegistered @event)
    {
        Id = @event.Id;
        Sku = @event.Sku;
        Name = @event.Name;
        Description = @event.Description;
        Status = @event.Status;
    }

    public void Apply(ProductModified @event)
    {
        Version++;

        Sku = @event.Sku;
        Name = @event.Name;
        Description = @event.Description;
    }

    public void Apply(ProductRemoved _)
    {
        Version++;

        Status = ProductStatus.Discontinue;
    }

    public override void When(object @event)
    {
        switch (@event)
        {
            case ProductRegistered registered:
                Apply(registered);
                return;
            case ProductModified modified:
                Apply(modified);
                return;
            case ProductRemoved productRemoved:
                Apply(productRemoved);
                return;
        }
    }
}

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