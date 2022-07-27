namespace Order.Orders.ProcessingOrder;

public record OrderProcessed(
    Guid OrderId,
    Guid PackageId,
    DateTime ProcessedAt
)
{
    public static OrderProcessed Create(Guid orderId, Guid packageId, DateTime processedAt)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (processedAt == default)
            throw new ArgumentOutOfRangeException(nameof(processedAt));

        return new(orderId, packageId, processedAt);
    }
}