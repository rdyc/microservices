using FW.Core.Events;

namespace Shipment.Packages.RequestingPackage;

public record PackagePrepared(
    Guid PackageId,
    Guid OrderId,
    DateTime PreparedAt
) : IExternalEvent
{
    public static PackagePrepared Create(
        Guid packageId,
        Guid orderId,
        DateTime preparedAt
    ) => new(packageId, orderId, preparedAt);
}