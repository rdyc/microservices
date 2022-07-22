using FW.Core.Events;
using Order.ShoppingCarts.FinalizingCart;

namespace Order.Shipments.SendingPackage;

public class PackageWasSent : IExternalEvent
{
    public Guid PackageId { get; }
    public Guid OrderId { get; }
    public IReadOnlyList<ShoppingCartProduct> Products { get; }
    public DateTime SentAt { get; }

    public PackageWasSent(
        Guid packageId,
        Guid orderId,
        IReadOnlyList<ShoppingCartProduct> productItems,
        DateTime sentAt)
    {
        OrderId = orderId;
        Products = productItems;
        SentAt = sentAt;
        PackageId = packageId;
    }
}