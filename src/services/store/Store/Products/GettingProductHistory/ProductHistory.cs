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
        AggregateId = @event.ProductId;
        Description = $"Product registered with id {@event.ProductId}";
    }

    public void Apply(ProductModified @event)
    {
        AggregateId = @event.ProductId;
        Description = $"Product modified with id {@event.ProductId}";
    }

    public void Apply(AttributeAdded @event)
    {
        AggregateId = @event.ProductId;
        Description = $"Product attribute added with id {@event.ProductId}";
    }

    public void Apply(AttributeRemoved @event)
    {
        AggregateId = @event.ProductId;
        Description = $"Product attribute removed with id {@event.ProductId}";
    }

    public void Apply(PriceChanged @event)
    {
        AggregateId = @event.ProductId;
        Description = $"Product price changed with id {@event.ProductId}";
    }

    public void Apply(StockChanged @event)
    {
        AggregateId = @event.ProductId;
        Description = $"Product stock changed with id {@event.ProductId}";
    }

    public void Apply(ProductSold @event)
    {
        AggregateId = @event.ProductId;
        Description = $"Product id {@event.ProductId} has been sold for {@event.Quantity} items";
    }

    public void Apply(ProductRemoved @event)
    {
        AggregateId = @event.ProductId;
        Description = $"Product removed with id {@event.ProductId}";
    }
}