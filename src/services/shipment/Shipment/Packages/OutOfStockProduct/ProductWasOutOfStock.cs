using Shipment.Products;

namespace Shipment.Packages.OutOfStockProduct;

public class ProductWasOutOfStock
{
    public Guid OrderId { get; }
    public IReadOnlyList<Product> AvailableProducts { get; }
    public DateTime AvailabilityCheckedAt { get; }


    public ProductWasOutOfStock(
        Guid orderId,
        IReadOnlyList<Product> availableProducts,
        DateTime availabilityCheckedAt
    )
    {
        OrderId = orderId;
        AvailableProducts = availableProducts;
        AvailabilityCheckedAt = availabilityCheckedAt;
    }
}