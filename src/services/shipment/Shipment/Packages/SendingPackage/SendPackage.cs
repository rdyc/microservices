using FW.Core.Commands;
using FW.Core.Requests;
using MediatR;
using Shipment;
using Shipment.Products;

namespace Order.Shipments.SendingPackage;

public record SendPackage(
    Guid OrderId,
    IEnumerable<Product> Products
) : ICommand
{
    public static SendPackage Create(
        Guid orderId,
        IEnumerable<Product> products
    )
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (products is null || !products.Any())
            throw new ArgumentOutOfRangeException(nameof(products));

        return new(orderId, products);
    }
}

internal class HandleSendPackage : ICommandHandler<SendPackage>
{
    private readonly ExternalServicesConfig config;
    private readonly IExternalCommandBus commandBus;

    public HandleSendPackage(
        ExternalServicesConfig config,
        IExternalCommandBus commandBus)
    {
        this.config = config;
        this.commandBus = commandBus;
    }

    public async Task<Unit> Handle(SendPackage request, CancellationToken cancellationToken)
    {
        await commandBus.Post(
            config.OrdersUrl!,
            "orders",
            request,
            cancellationToken);

        return Unit.Value;
    }
}