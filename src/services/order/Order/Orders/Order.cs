using FW.Core.Aggregates;
using Order.Orders.CancellingOrder;
using Order.Orders.CompletingOrder;
using Order.Orders.InitializingOrder;
using Order.Orders.RecordingOrderPayment;
using Order.ShoppingCarts.FinalizingCart;

namespace Order.Orders;

public class Order : Aggregate
{
    public Guid? ClientId { get; private set; }
    public IEnumerable<ShoppingCartProduct> Products { get; private set; } = default!;
    public decimal TotalPrice { get; private set; } = 0;
    public OrderStatus Status { get; private set; }
    public Guid? PaymentId { get; private set; }

    public static Order Initialize(
        Guid orderId,
        Guid clientId,
        IEnumerable<ShoppingCartProduct> products,
        decimal totalPrice)
    {
        return new Order(
            orderId,
            clientId,
            products,
            totalPrice
        );
    }

    public Order() { }

    private Order(Guid id, Guid clientId, IEnumerable<ShoppingCartProduct> products, decimal totalPrice)
    {
        var @event = OrderInitialized.Create(
            id,
            clientId,
            products,
            totalPrice,
            DateTime.UtcNow
        );

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(OrderInitialized @event)
    {
        Version++;

        Id = @event.OrderId;
        ClientId = @event.ClientId;
        Products = @event.Products;
        Status = OrderStatus.Opened;
    }

    public void RecordPayment(Guid paymentId, DateTime recordedAt)
    {
        var @event = OrderPaymentRecorded.Create(
            Id,
            paymentId,
            Products,
            TotalPrice,
            recordedAt
        );

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(OrderPaymentRecorded @event)
    {
        Version++;

        PaymentId = @event.PaymentId;
        Status = OrderStatus.Paid;
    }

    public void Complete()
    {
        if (Status != OrderStatus.Paid)
            throw new InvalidOperationException($"Cannot complete a not paid order.");

        var @event = OrderCompleted.Create(Id, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(OrderCompleted _)
    {
        Version++;

        Status = OrderStatus.Completed;
    }

    public void Cancel(OrderCancellationReason cancellationReason)
    {
        if (OrderStatus.Closed.HasFlag(Status))
            throw new InvalidOperationException($"Cannot cancel a closed order.");

        var @event = OrderCancelled.Create(Id, PaymentId, cancellationReason, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(OrderCancelled _)
    {
        Version++;

        Status = OrderStatus.Cancelled;
    }
}