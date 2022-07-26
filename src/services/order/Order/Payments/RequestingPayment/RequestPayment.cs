using FW.Core.Commands;
using FW.Core.Requests;
using MediatR;

namespace Order.Payments.RequestingPayment;

public class RequestPayment : ICommand
{
    public Guid OrderId { get; }
    public decimal Amount { get; }

    private RequestPayment(Guid orderId, decimal amount)
    {
        OrderId = orderId;
        Amount = amount;
    }

    public static RequestPayment Create(Guid orderId, decimal amount)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        return new(orderId, amount);
    }
}

public class HandleRequestPayment : ICommandHandler<RequestPayment>
{
    private readonly ExternalServicesConfig config;
    private readonly IExternalCommandBus commandBus;

    public HandleRequestPayment(
        ExternalServicesConfig config,
        IExternalCommandBus commandBus)
    {
        this.config = config;
        this.commandBus = commandBus;
    }

    public async Task<Unit> Handle(RequestPayment request, CancellationToken cancellationToken)
    {
        await commandBus.Post(
            config.PaymentsUrl!,
            "payments",
            request,
            cancellationToken);

        return Unit.Value;
    }
}