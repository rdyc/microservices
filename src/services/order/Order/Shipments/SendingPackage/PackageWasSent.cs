namespace Order.Shipments.SendingPackage;

public record PackageWasSent(
    Guid PackageId,
    Guid OrderId,
    DateTime SentAt
);