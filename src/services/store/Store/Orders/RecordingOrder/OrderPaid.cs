using FW.Core.Commands;
using FW.Core.Events;
using Store.Products.SellingProduct;

namespace Store.Orders.RecordingOrder;

public record OrderPaid(
    Guid OrderId,
    Guid PaymentId,
    IEnumerable<OrderedProduct> Products
)
{
    public static OrderPaid Create(Guid orderId, Guid paymentId, IEnumerable<OrderedProduct> products) =>
        new(orderId, paymentId, products);
}

public record OrderedProduct(
    Guid ProductId,
    int Quantity
)
{
    public static OrderedProduct Create(Guid productId, int quantity) =>
        new(productId, quantity);
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
            await commandBus.SendAsync(
                SellProduct.Create(product.ProductId, product.Quantity), cancellationToken);
        }
    }
}