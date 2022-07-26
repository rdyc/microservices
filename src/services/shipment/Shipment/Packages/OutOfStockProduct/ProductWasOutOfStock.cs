using Shipment.Products;

namespace Shipment.Packages.OutOfStockProduct;

public record ProductWasOutOfStock(
    Guid OrderId,
    IEnumerable<Product> AvailableProducts,
    DateTime AvailabilityCheckedAt
)
{
    public static ProductWasOutOfStock Create(
        Guid orderId,
        IReadOnlyList<Product> availableProducts,
        DateTime availabilityCheckedAt
    ) => new(orderId, availableProducts, availabilityCheckedAt);
}