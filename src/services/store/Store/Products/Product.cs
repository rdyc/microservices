using FW.Core.Aggregates;
using FW.Core.Extensions;
using Store.Currencies;
using Store.Products.AddingAttribute;
using Store.Products.ModifyingProduct;
using Store.Products.RegisteringProduct;
using Store.Products.RemovingAttribute;
using Store.Products.RemovingProduct;
using Store.Products.UpdatingPrice;
using Store.Products.UpdatingStock;

namespace Store.Products;

public class Product : Aggregate
{
    public string SKU { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public IList<ProductAttribute> Attributes { get; private set; } = default!;
    public Currency Currency { get; private set; } = default!;
    public decimal Price { get; private set; } = default!;
    public int Stock { get; private set; } = default!;
    public ProductStatus Status { get; private set; } = default!;

    public static Product Register(Guid? id, string sku, string name, string description)
    {
        if (id is null)
            throw new ArgumentNullException(nameof(id));

        return new Product(id.Value, sku, name, description);
    }

    private Product(Guid id, string sku, string name, string description)
    {
        var evt = ProductRegistered.Create(id, sku, name, description);

        Enqueue(evt);
        Apply(evt);
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
            case AttributeAdded attributeAdded:
                Apply(attributeAdded);
                return;
            case AttributeRemoved attributeRemoved:
                Apply(attributeRemoved);
                return;
            case PriceChanged priceChanged:
                Apply(priceChanged);
                return;
            case StockChanged stockChanged:
                Apply(stockChanged);
                return;
            case ProductRemoved productRemoved:
                Apply(productRemoved);
                return;
        }
    }

    public void Apply(ProductRegistered evt)
    {
        Id = evt.Id;
        SKU = evt.SKU;
        Name = evt.Name;
        Description = evt.Description;
        Status = ProductStatus.Available;
    }

    public void Modify(string sku, string name, string description)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"The product was discontinued");

        var evt = ProductModified.Create(sku, name, description);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(ProductModified evt)
    {
        Version++;

        SKU = evt.SKU;
        Name = evt.Name;
        Description = evt.Description;
    }

    public void AddAttribute(Guid attributeId, string value)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"Adding attribute for the product in '{Status}' status is not allowed.");

        var @event = AttributeAdded.Create(attributeId, value);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(AttributeAdded @event)
    {
        var (attributeId, value) = @event;
        var newAttribute = ProductAttribute.From(attributeId, value) ;
        var existingAttribute = FindAttributeMatchingWith(newAttribute);

        if (existingAttribute is null)
        {
            Attributes.Add(newAttribute);
            return;
        }

        Attributes.Replace(
            existingAttribute,
            existingAttribute.MergeWith(newAttribute)
        );
    }

    public void RemoveAttribute(Guid attributeId)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"Adding attribute for the product in '{Status}' status is not allowed.");

        var existingAttribute = FindAttributeMatchingWith(attributeId);

        if (existingAttribute is null)
            throw new InvalidOperationException($"An attribute with id `{attributeId}` was not found in product.");

        var @event = AttributeRemoved.Create(attributeId);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(AttributeRemoved @event)
    {
        var existingAttribute = FindAttributeMatchingWith(@event.AttributeId);

        if (existingAttribute == null)
            return;

        if (existingAttribute.HasTheSameValue(existingAttribute))
        {
            Attributes.Remove(existingAttribute);
        }
    }

    public void UpdatePrice(Currency currency, decimal price)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"The product has discontinued");

        var evt = PriceChanged.Create(currency, price);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(PriceChanged evt)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"The product has discontinued");

        Version++;

        Currency = evt.Currency;
        Price = evt.Price;
    }

    public void UpdateStock(int stock)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"The product has discontinued");

        var evt = StockChanged.Create(stock);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(StockChanged evt)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"The product has discontinued");

        Version++;

        Stock = evt.Stock;
    }

    public void Remove()
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"The product was discontiued");

        var evt = ProductRemoved.Create(Id);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(ProductRemoved _)
    {
        Version++;

        Status = ProductStatus.Discontinue;
    }

    private ProductAttribute? FindAttributeMatchingWith(Guid attributeId)
    {
        return Attributes.SingleOrDefault(e => e.MatchesAttribute(attributeId));
    }

    private ProductAttribute? FindAttributeMatchingWith(ProductAttribute productAttribute)
    {
        return Attributes.SingleOrDefault(e => e.MatchesAttributeAndValue(productAttribute));
    }
}