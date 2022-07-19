using FW.Core.MongoDB;
using FW.Core.Projections;
using MongoDB.Bson.Serialization.Attributes;
using Store.Products.AddingAttribute;
using Store.Products.ModifyingProduct;
using Store.Products.RegisteringProduct;
using Store.Products.RemovingAttribute;
using Store.Products.RemovingProduct;
using Store.Products.SellingProduct;
using Store.Products.UpdatingPrice;
using Store.Products.UpdatingStock;

namespace Store.Products.GettingProductHistory;

[BsonCollection("product_history")]
public record ProductHistory : Document, IVersionedProjection
{
    [BsonElement("aggregate_id")]
    public Guid AggregateId { get; set; } = default!;

    [BsonElement("description")]
    public string Description { get; set; } = default!;

    [BsonElement("last_position")]
    public ulong LastProcessedPosition { get; set; } = default!;

    public void When(object @event)
    {
        switch (@event)
        {
            case ProductRegistered productRegistered:
                Apply(productRegistered);
                return;
            case ProductModified productModified:
                Apply(productModified);
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
            case ProductRemoved productRemoved:
                Apply(productRemoved);
                return;
        }
    }

    public void Apply(ProductRegistered @event)
    {
        AggregateId = @event.Id;
        Description = $"Product registered with id {@event.Id}";
    }

    public void Apply(ProductModified @event)
    {
        AggregateId = @event.Id;
        Description = $"Product modified with id {@event.Id}";
    }

    public void Apply(ProductAttributeAdded @event)
    {
        AggregateId = @event.Id;
        Description = $"Product attribute added with id {@event.Id}";
    }

    public void Apply(ProductAttributeRemoved @event)
    {
        AggregateId = @event.Id;
        Description = $"Product attribute removed with id {@event.Id}";
    }

    public void Apply(ProductPriceChanged @event)
    {
        AggregateId = @event.Id;
        Description = $"Product price changed with id {@event.Id}";
    }

    public void Apply(ProductStockChanged @event)
    {
        AggregateId = @event.Id;
        Description = $"Product stock changed with id {@event.Id}";
    }

    public void Apply(ProductSold @event)
    {
        AggregateId = @event.Id;
        Description = $"Product id {@event.Id} has been sold for {@event.Quantity} items";
    }

    public void Apply(ProductRemoved @event)
    {
        AggregateId = @event.Id;
        Description = $"Product removed with id {@event.Id}";
    }
}