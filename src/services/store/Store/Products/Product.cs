using FW.Core.Aggregates;
using FW.Core.Extensions;
using Store.Products.AddingAttribute;
using Store.Products.ModifyingProduct;
using Store.Products.RegisteringProduct;
using Store.Products.RemovingAttribute;
using Store.Products.RemovingProduct;
using Store.Products.SellingProduct;
using Store.Products.ShippingProduct;
using Store.Products.UpdatingPrice;
using Store.Products.UpdatingStock;

namespace Store.Products;

public class Product : Aggregate
{
    public string Sku { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public IList<ProductAttribute> Attributes { get; private set; } = default!;
    public ProductCurrency Currency { get; private set; } = default!;
    public decimal Price { get; private set; } = default!;
    public int Sold { get; private set; } = default!;
    public int Stock { get; private set; } = default!;
    public ProductStatus Status { get; private set; } = default!;

    private Product() { }

    public static Product Register(Guid? id, string sku, string name, string description)
    {
        if (id is null)
            throw new ArgumentNullException(nameof(id));

        return new Product(id.Value, sku, name, description, ProductStatus.Available);
    }

    private Product(Guid id, string sku, string name, string description, ProductStatus status)
    {
        var evt = ProductRegistered.Create(id, sku, name, description, status);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(ProductRegistered evt)
    {
        Id = evt.ProductId;
        Sku = evt.Sku;
        Name = evt.Name;
        Description = evt.Description;
        Status = evt.Status;
    }

    public void Modify(string sku, string name, string description)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"The product was discontinued");

        var evt = ProductModified.Create(Id, sku, name, description);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(ProductModified evt)
    {
        Version++;

        Sku = evt.Sku;
        Name = evt.Name;
        Description = evt.Description;
    }

    public void AddAttribute(ProductAttribute productAttribute)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"Adding attribute for the product in '{Status}' status is not allowed.");

        var @event = ProductAttributeAdded.Create(Id, productAttribute);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(ProductAttributeAdded @event)
    {
        Version++;

        if (Attributes is null)
            Attributes = new List<ProductAttribute>();

        var (_, id, name, type, unit, value) = @event;
        var newAttribute = ProductAttribute.Create(id, name, type, unit, value);
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

        var @event = ProductAttributeRemoved.Create(Id, existingAttribute);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(ProductAttributeRemoved @event)
    {
        Version++;

        if (Attributes is null)
            return;

        var existingAttribute = FindAttributeMatchingWith(@event.AttributeId);

        if (existingAttribute == null)
            return;

        if (existingAttribute.HasTheSameValue(existingAttribute))
        {
            Attributes.Remove(existingAttribute);
        }
    }

    public void UpdatePrice(ProductCurrency currency, decimal price)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"The product has discontinued");

        var evt = ProductPriceChanged.Create(Id, currency, price);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(ProductPriceChanged evt)
    {
        Version++;

        Currency = evt.Currency;
        Price = evt.Price;
    }

    public void UpdateStock(int stock)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"The product has discontinued");

        var evt = ProductStockChanged.Create(Id, stock);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(ProductStockChanged evt)
    {
        Version++;

        Stock += evt.Stock;
    }

    public void SoldFor(int quantity)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"The product has discontinued");

        var evt = ProductSold.Create(Id, quantity);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(ProductSold _)
    {
        Version++;

        Sold++;
    }

    public void PullStock(int quantity)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"The product has discontinued");

        var evt = ProductShipped.Create(Id, quantity);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(ProductShipped evt)
    {
        Version++;

        Stock -= evt.Quantity;
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
            case ProductAttributeAdded attributeAdded:
                Apply(attributeAdded);
                return;
            case ProductAttributeRemoved attributeRemoved:
                Apply(attributeRemoved);
                return;
            case ProductPriceChanged priceChanged:
                Apply(priceChanged);
                return;
            case ProductStockChanged stockChanged:
                Apply(stockChanged);
                return;
            case ProductSold productSold:
                Apply(productSold);
                return;
            case ProductShipped productShipped:
                Apply(productShipped);
                return;
            case ProductRemoved productRemoved:
                Apply(productRemoved);
                return;
        }
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