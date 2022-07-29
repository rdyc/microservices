using EventStore.Client;
using FW.Core.Commands;
using FW.Core.Events;
using FW.Core.EventStoreDB.Events;
using Order.Orders.CancellingOrder;
using Order.Orders.CompletingOrder;
using Order.Orders.InitializingOrder;
using Order.Orders.ProcessingOrder;
using Order.Orders.RecordingOrderPayment;
using Order.Payments.DiscardingPayment;
using Order.Payments.FailingPayment;
using Order.Payments.FinalizingPayment;
using Order.Payments.RequestingPayment;
using Order.Shipments.DiscardingPackage;
using Order.Shipments.RequestingPackage;
using Order.Shipments.SendingPackage;
using Order.Carts.FinalizingCart;

namespace Order.Orders;

public class OrderSaga :
    IEventHandler<CartFinalized>,
    IEventHandler<OrderInitialized>,
    IEventHandler<PaymentFinalized>,
    IEventHandler<PaymentFailed>,
    IEventHandler<PackagePrepared>,
    IEventHandler<PackageWasSent>,
    IEventHandler<ProductWasOutOfStock>,
    IEventHandler<OrderCancelled>
{
    private readonly EventStoreClient eventStore;
    private readonly ICommandBus commandBus;

    public OrderSaga(
        EventStoreClient eventStore,
        ICommandBus commandBus)
    {
        this.eventStore = eventStore;
        this.commandBus = commandBus;
    }

    public async Task Handle(CartFinalized @event, CancellationToken cancellationToken)
    {
        var (cartId, clientId, products, totalPrice, _) = @event;

        var order = await eventStore.AggregateStream<Order>(cartId, cancellationToken);

        if (order is null)
        {
            await commandBus.SendAsync(
                InitializeOrder.Create(cartId, clientId, products, totalPrice),
                cancellationToken);
        }
    }

    public async Task Handle(OrderInitialized @event, CancellationToken cancellationToken)
    {
        await commandBus.SendAsync(
            RequestPayment.Create(@event.OrderId, @event.TotalPrice),
            cancellationToken);
    }

    public async Task Handle(PaymentFinalized @event, CancellationToken cancellationToken)
    {
        await commandBus.SendAsync(
            RecordOrderPayment.Create(@event.OrderId, @event.PaymentId, @event.FinalizedAt),
            cancellationToken);
    }
    
    public async Task Handle(PaymentFailed @event, CancellationToken cancellationToken)
    {
        await commandBus.SendAsync(
            CancelOrder.Create(@event.OrderId, OrderCancellationReason.TimedOut, @event.FailedAt),
            cancellationToken);
    }

    public async Task Handle(PackagePrepared @event, CancellationToken cancellationToken)
    {
        await commandBus.SendAsync(
            ProcessOrder.Create(@event.OrderId, @event.PackageId, @event.PreparedAt),
            cancellationToken);
    }

    public async Task Handle(PackageWasSent @event, CancellationToken cancellationToken)
    {
        await commandBus.SendAsync(
            CompleteOrder.Create(@event.OrderId, DateTime.UtcNow),
            cancellationToken);
    }

    public async Task Handle(ProductWasOutOfStock @event, CancellationToken cancellationToken)
    {
        await commandBus.SendAsync(
            CancelOrder.Create(@event.OrderId, OrderCancellationReason.ProductWasOutOfStock, @event.AvailabilityCheckedAt),
            cancellationToken);
    }

    public async Task Handle(OrderCancelled @event, CancellationToken cancellationToken)
    {
        if (@event.PaymentId.HasValue)
            await commandBus.SendAsync(
                DiscardPayment.Create(@event.PaymentId.Value),
                cancellationToken);
        else
            await Task.CompletedTask;
    }
}