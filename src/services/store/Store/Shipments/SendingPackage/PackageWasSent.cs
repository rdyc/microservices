using FW.Core.Commands;
using FW.Core.Events;
using Store.Products.ShippingProduct;

namespace Store.Shipments.SendingPackage;

public record PackageWasSent(
    Guid PackageId,
    Guid OrderId,
    IEnumerable<PackageItem> Items,
    DateTime SentAt
);

public record PackageItem(
    Guid ProductId,
    int Quantity
);

internal class HandlePackageShipment : IEventHandler<EventEnvelope<PackageWasSent>>
{
    private readonly ICommandBus commandBus;

    public HandlePackageShipment(ICommandBus commandBus)
    {
        this.commandBus = commandBus;
    }

    public async Task Handle(EventEnvelope<PackageWasSent> @event, CancellationToken cancellationToken)
    {
        var items = new List<PackageItem>();
        items.Add(new PackageItem(Guid.Parse("7d348ef4-8d90-40db-8bf5-40bf2122dac1"), 2));
        
        foreach (var product in items)
        {
            await commandBus.SendAsync(
                ShipProduct.Create(product.ProductId, product.Quantity),
                cancellationToken);
        }
    }
}