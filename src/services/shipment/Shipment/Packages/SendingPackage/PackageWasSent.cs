using FW.Core.Events;
using Shipment.Products;

namespace Shipment.Packages.SendingPackage;

public class PackageWasSent : IExternalEvent
{
    public Guid PackageId { get; }
    public Guid OrderId { get; }
    public IReadOnlyList<Product> Products { get; }
    public DateTime SentAt { get; }

    public PackageWasSent(
        Guid packageId,
        Guid orderId,
        IReadOnlyList<Product> products,
        DateTime sentAt)
    {
        OrderId = orderId;
        Products = products;
        SentAt = sentAt;
        PackageId = packageId;
    }
}