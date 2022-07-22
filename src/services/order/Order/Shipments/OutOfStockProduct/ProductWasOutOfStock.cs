using Order.ShoppingCarts.FinalizingCart;

namespace Order.Shipments.OutOfStockProduct;

public class ProductWasOutOfStock
{
    public Guid OrderId { get; }
    public IReadOnlyList<ShoppingCartProduct> AvailableProducts { get; }
    public DateTime AvailabilityCheckedAt { get; }


    public ProductWasOutOfStock(
        Guid orderId,
        IReadOnlyList<ShoppingCartProduct> availableProducts,
        DateTime availabilityCheckedAt
    )
    {
        OrderId = orderId;
        AvailableProducts = availableProducts;
        AvailabilityCheckedAt = availabilityCheckedAt;
    }
}