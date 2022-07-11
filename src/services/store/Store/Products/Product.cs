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
    public IList<ProductAttribute> ProductAttributes { get; private set; } = default!;
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

        var evt = ProductModified.Create(Id, sku, name, description);

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

    public void AddAttribute(ProductAttribute productAttribute)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"Adding attribute for the product in '{Status}' status is not allowed.");

        var @event = AttributeAdded.Create(Id, productAttribute);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(AttributeAdded @event)
    {
        var newProductAttribute = @event.ProductAttribute;

        var existingProductAttribute = FindAttributeMatchingWith(newProductAttribute);

        if (existingProductAttribute is null)
        {
            ProductAttributes.Add(newProductAttribute);
            return;
        }

        ProductAttributes.Replace(
            existingProductAttribute,
            existingProductAttribute.MergeWith(newProductAttribute)
        );
    }

    public void RemoveAttribute(ProductAttribute productAttribute)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"Adding attribute for the product in '{Status}' status is not allowed.");

        var existingProductAttribute = FindAttributeMatchingWith(productAttribute);

        if (existingProductAttribute is null)
            throw new InvalidOperationException($"Product attribute with id `{productAttribute.AttributeId}` was not found in cart.");

        var @event = AttributeRemoved.Create(Id, productAttribute);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(AttributeRemoved @event)
    {
        var productAttributeToBeRemoved = @event.ProductAttribute;

        var existingProductAttribute = FindAttributeMatchingWith(productAttributeToBeRemoved);

        if (existingProductAttribute == null)
            return;

        if (existingProductAttribute.HasTheSameValue(productAttributeToBeRemoved))
        {
            ProductAttributes.Remove(existingProductAttribute);
            return;
        }

        ProductAttributes.Replace(
            existingProductAttribute,
            existingProductAttribute.Subtract(productAttributeToBeRemoved)
        );
    }

    public void UpdatePrice(Currency currency, decimal price)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"The product has discontinued");

        var evt = PriceChanged.Create(Id, currency, price);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(PriceChanged evt)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"The product has discountinued");

        Version++;

        Currency = evt.Currency;
        Price = evt.Price;
    }

    public void UpdateStock(int stock)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"The product has discontinued");

        var evt = StockChanged.Create(Id, stock);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(StockChanged evt)
    {
        if (Status == ProductStatus.Discontinue)
            throw new InvalidOperationException($"The product has discountinued");

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

    private ProductAttribute? FindAttributeMatchingWith(ProductAttribute productAttribute)
    {
        return ProductAttributes.SingleOrDefault(e => e.MatchesAttribute(productAttribute));
    }
}