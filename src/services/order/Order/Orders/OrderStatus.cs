namespace Order.Orders;

public enum OrderStatus
{
    Opened = 1,
    Paid = 2,
    Processed = 4,
    Completed = 6,
    Cancelled = 8,
    Closed = Completed | Cancelled
}