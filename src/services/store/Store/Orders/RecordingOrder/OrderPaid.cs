using FW.Core.Commands;
using FW.Core.Events;
using Store.Products.SellingProduct;

namespace Store.Orders.RecordingOrder;

public class OrderPaid : IExternalEvent
{
    public OrderPaid(Guid orderId, Guid paymentId, IEnumerable<OrderProduct> products)
    {
        OrderId = orderId;
        PaymentId = paymentId;
        Products = products;
    }

    public Guid OrderId { get; }
    public Guid PaymentId { get; }
    public IEnumerable<OrderProduct> Products { get; }
}

public class OrderProduct
{
    public OrderProduct(Guid productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }

    public Guid ProductId { get; }
    public int Quantity { get; }
}

internal class HandleOrderPaid : IEventHandler<EventEnvelope<OrderPaid>>
{
    private readonly ICommandBus commandBus;

    public HandleOrderPaid(ICommandBus commandBus)
    {
        this.commandBus = commandBus;
    }

    public async Task Handle(EventEnvelope<OrderPaid> @event, CancellationToken cancellationToken)
    {
        foreach (var product in @event.Data.Products)
        {
            await commandBus.Send(SellProduct.Create(product.ProductId, product.Quantity));
        }
    }
}