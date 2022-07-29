using FW.Core.Commands;
using FW.Core.Events;
using Store.Products.SellingProduct;
using Store.Carts;

namespace Store.Orders.RecordingOrderPayment;

public record OrderPaymentRecorded(
    Guid ClientId,
    Guid OrderId,
    Guid PaymentId,
    IEnumerable<CartProduct> Products,
    decimal TotalPrice,
    DateTime RecordedAt
);

internal class HandleOrderPaymentRecorded : IEventHandler<EventEnvelope<OrderPaymentRecorded>>
{
    private readonly ICommandBus commandBus;

    public HandleOrderPaymentRecorded(ICommandBus commandBus)
    {
        this.commandBus = commandBus;
    }

    public async Task Handle(EventEnvelope<OrderPaymentRecorded> @event, CancellationToken cancellationToken)
    {
        foreach (var product in @event.Data.Products)
        {
            await commandBus.SendAsync(
                SellProduct.Create(product.ProductId, product.Quantity),
                cancellationToken);
        }
    }
}