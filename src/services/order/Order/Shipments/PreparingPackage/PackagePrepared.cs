namespace Order.Shipments.RequestingPackage;

public record PackagePrepared(
    Guid PackageId,
    Guid OrderId,
    DateTime PreparedAt
);