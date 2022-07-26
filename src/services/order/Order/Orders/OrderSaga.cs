using FW.Core.Commands;
using FW.Core.Events;
using Order.Orders.CancellingOrder;
using Order.Orders.CompletingOrder;
using Order.Orders.InitializingOrder;
using Order.Orders.RecordingOrderPayment;
using Order.Payments.DiscardingPayment;
using Order.Payments.FinalizingPayment;
using Order.Payments.RequestingPayment;
using Order.Shipments.OutOfStockProduct;
using Order.Shipments.SendingPackage;
using Order.ShoppingCarts.FinalizingCart;

namespace Order.Orders;

public class OrderSaga :
    IEventHandler<ShoppingCartFinalized>,
    IEventHandler<OrderInitialized>,
    IEventHandler<PaymentFinalized>,
    IEventHandler<PackageWasSent>,
    IEventHandler<ProductWasOutOfStock>,
    IEventHandler<OrderCancelled>,
    IEventHandler<OrderPaymentRecorded>
{
    private readonly ICommandBus commandBus;

    public OrderSaga(ICommandBus commandBus)
    {
        this.commandBus = commandBus;
    }

    // Happy path
    public async Task Handle(ShoppingCartFinalized @event, CancellationToken cancellationToken)
    {
        var orderId = Guid.NewGuid();

        await commandBus.SendAsync(
            InitializeOrder.Create(orderId, @event.ClientId, @event.Products, @event.TotalPrice),
            cancellationToken);
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

    public async Task Handle(OrderPaymentRecorded @event, CancellationToken cancellationToken)
    {
        await commandBus.SendAsync(
            SendPackage.Create(@event.OrderId, @event.Products),
            cancellationToken
        );
    }

    public async Task Handle(PackageWasSent @event, CancellationToken cancellationToken)
    {
        await commandBus.SendAsync(
            CompleteOrder.Create(@event.OrderId),
            cancellationToken);
    }

    // Compensation
    public async Task Handle(ProductWasOutOfStock @event, CancellationToken cancellationToken)
    {
        await commandBus.SendAsync(
            CancelOrder.Create(@event.OrderId, OrderCancellationReason.ProductWasOutOfStock),
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