using FW.Core.Commands;
using FW.Core.Events;
using Store.Products.SellingProduct;

namespace Store.Orders.RecordingOrderPayment;

public record OrderPaymentRecorded(
    Guid OrderId,
    Guid PaymentId,
    IEnumerable<ShoppingCartProduct> Products,
    decimal Amount,
    DateTime RecordedAt
);

public record ShoppingCartProduct(
    Guid ProductId,
    string Sku,
    string Name,
    int Quantity,
    ShoppingCartCurrency Currency,
    decimal Price
);

public record ShoppingCartCurrency(
    Guid Id,
    string Name,
    string Code,
    string Symbol
);

internal class HandleOrderPaid : IEventHandler<EventEnvelope<OrderPaymentRecorded>>
{
    private readonly ICommandBus commandBus;

    public HandleOrderPaid(ICommandBus commandBus)
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