using FW.Core.Events;

namespace Shipment.Packages.SendingPackage;

public record PackageWasSent(
    Guid PackageId,
    Guid OrderId,
    IEnumerable<PackageItem> Items,
    DateTime SentAt
) : IExternalEvent
{
    public static PackageWasSent Create(
        Guid packageId,
        Guid orderId,
        IEnumerable<PackageItem> items,
        DateTime sentAt
    ) => new(packageId, orderId, items, sentAt);
}