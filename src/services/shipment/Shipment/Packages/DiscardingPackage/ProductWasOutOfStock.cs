using FW.Core.Events;

namespace Shipment.Packages.DiscardingPackage;

public record ProductWasOutOfStock(
    Guid PackageId,
    Guid OrderId,
    DateTime AvailabilityCheckedAt
) : IExternalEvent
{
    public static ProductWasOutOfStock Create(
        Guid packageId,
        Guid orderId,
        DateTime availabilityCheckedAt
    ) => new(packageId, orderId, availabilityCheckedAt);
}