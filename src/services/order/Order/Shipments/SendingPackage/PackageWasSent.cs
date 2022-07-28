namespace Order.Shipments.SendingPackage;

public record PackageWasSent(
    Guid PackageId,
    Guid OrderId,
    IEnumerable<PackageItem> Items,
    DateTime SentAt
);

public record PackageItem(
    Guid ProductId,
    int Quantity
);