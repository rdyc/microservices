using FW.Core.Aggregates;
using Order.Orders.CancellingOrder;
using Order.Orders.CompletingOrder;
using Order.Orders.InitializingOrder;
using Order.Orders.ProcessingOrder;
using Order.Orders.RecordingOrderPayment;
using Order.ShoppingCarts.FinalizingCart;

namespace Order.Orders;

public class Order : Aggregate
{
    private Order(){}

    public Guid ClientId { get; private set; }
    public IList<ShoppingCartProduct> Products { get; private set; } = default!;
    public decimal TotalPrice { get; private set; } = 0;
    public OrderStatus Status { get; private set; }
    public Guid? PaymentId { get; private set; }
    public Guid? PackageId { get; private set; }

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

    public void Apply(OrderInitialized @event)
    {
        Version = 0;
        Id = @event.OrderId;
        ClientId = @event.ClientId;
        Products = @event.Products.ToList();
        Status = OrderStatus.Opened;
    }

    public void Process(Guid packageId, DateTime processedAt)
    {
        if (Status != OrderStatus.Paid)
            throw new InvalidOperationException($"Process order in '{Status}' is not allowed.");

        var @event = OrderProcessed.Create(Id, packageId, processedAt);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(OrderProcessed @event)
    {
        Version++;

        PackageId = @event.PackageId;
        Status = OrderStatus.Processed;
    }

    public void RecordPayment(Guid paymentId, DateTime recordedAt)
    {
        if (Status != OrderStatus.Opened)
            throw new InvalidOperationException($"Record payment in '{Status}' status is not allowed.");

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

    public void Complete(DateTime completedAt)
    {
        if (Status != OrderStatus.Processed)
            throw new InvalidOperationException($"Complete order in '{Status}' is not allowed.");

        var @event = OrderCompleted.Create(Id, completedAt);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(OrderCompleted _)
    {
        Version++;

        Status = OrderStatus.Completed;
    }

    public void Cancel(OrderCancellationReason cancellationReason, DateTime cancelledAt)
    {
        if (OrderStatus.Closed.HasFlag(Status))
            throw new InvalidOperationException($"Cannot cancel a closed order.");

        var @event = OrderCancelled.Create(Id, PaymentId, cancellationReason, cancelledAt);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(OrderCancelled _)
    {
        Version++;

        Status = OrderStatus.Cancelled;
    }

    public override void When(object @event)
    {
        switch (@event)
        {
            case OrderInitialized initialized:
                Apply(initialized);
                return;
            case OrderPaymentRecorded paymentRecorded:
                Apply(paymentRecorded);
                return;
            case OrderProcessed processed:
                Apply(processed);
                return;
            case OrderCompleted completed:
                Apply(completed);
                return;
            case OrderCancelled cancelled:
                Apply(cancelled);
                return;
        }
    }
}