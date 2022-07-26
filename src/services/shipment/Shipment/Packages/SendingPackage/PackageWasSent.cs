using FW.Core.Events;

namespace Shipment.Packages.SendingPackage;

public record PackageWasSent(
    Guid PackageId,
    Guid OrderId,
    DateTime SentAt
) : IExternalEvent
{
    public static PackageWasSent Create(
        Guid packageId,
        Guid orderId,
        DateTime sentAt
    ) => new(packageId, orderId, sentAt);
}