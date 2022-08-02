using FW.Core.Aggregates;
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
    public bool IsActive { get; private set; } = true;

    public Product() { }

    [JsonConstructor]
    public Product(Guid id, string sku, string name, string description)
    {
        Id = id;
        Sku = sku;
        Name = name;
        Description = description;
    }

    public void Apply(ProductRegistered evt)
    {
        Id = evt.Id;
        Sku = evt.Sku;
        Name = evt.Name;
        Description = evt.Description;
    }

    public void Apply(ProductModified evt)
    {
        Version++;

        Sku = evt.Sku;
        Name = evt.Name;
        Description = evt.Description;
    }

    public void Apply(ProductRemoved _)
    {
        Version++;

        IsActive = false;
    }

    public override void When(object evt)
    {
        switch (evt)
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