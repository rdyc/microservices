namespace Order.Shipments.DiscardingPackage;

public record ProductWasOutOfStock(
    Guid PackageId,
    Guid OrderId,
    DateTime AvailabilityCheckedAt
);