using FW.Core.Commands;
using MediatR;
using Order.ShoppingCarts.FinalizingCart;

namespace Order.Shipments.SendingPackage;

public class SendPackage : ICommand
{
    public Guid OrderId { get; }
    public IEnumerable<ShoppingCartProduct> Products { get; }

    private SendPackage(
        Guid orderId,
        IEnumerable<ShoppingCartProduct> products
    )
    {
        OrderId = orderId;
        Products = products;
    }

    public static SendPackage Create(
        Guid orderId,
        IEnumerable<ShoppingCartProduct> products
    )
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (products is null || !products.Any())
            throw new ArgumentOutOfRangeException(nameof(products));

        return new(orderId, products);
    }
}

public class HandleSendPackage : ICommandHandler<SendPackage>
{
    /* private readonly ExternalServicesConfig externalServicesConfig;
    private readonly IExternalCommandBus externalCommandBus;

    public HandleSendPackage(
        ExternalServicesConfig externalServicesConfig,
        IExternalCommandBus externalCommandBus)
    {
        this.externalServicesConfig = externalServicesConfig;
        this.externalCommandBus = externalCommandBus;
    } */

    public async Task<Unit> Handle(SendPackage command, CancellationToken cancellationToken)
    {
        /* await externalCommandBus.Post(
            externalServicesConfig.ShipmentsUrl!,
            "shipments",
            command,
            cancellationToken); */

        return Unit.Value;
    }
}