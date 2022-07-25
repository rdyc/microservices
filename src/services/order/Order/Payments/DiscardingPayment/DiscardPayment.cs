using FW.Core.Commands;
using FW.Core.Requests;
using MediatR;

namespace Order.Payments.DiscardingPayment;

public class DiscardPayment : ICommand
{
    public Guid PaymentId { get; }
    public DiscardReason DiscardReason { get; }

    private DiscardPayment(Guid paymentId, DiscardReason discardReason)
    {
        PaymentId = paymentId;
        DiscardReason = discardReason;
    }

    public static DiscardPayment Create(Guid paymentId)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));

        return new DiscardPayment(paymentId, DiscardReason.OrderCancelled);
    }
}


public class HandleDiscardPayment : ICommandHandler<DiscardPayment>
{
    private readonly ExternalServicesConfig config;
    private readonly IExternalCommandBus commandBus;

    public HandleDiscardPayment(ExternalServicesConfig config,
        IExternalCommandBus commandBus)
    {
        this.config = config;
        this.commandBus = commandBus;
    }

    public async Task<Unit> Handle(DiscardPayment command, CancellationToken cancellationToken)
    {
        await commandBus.Delete(
            config.PaymentsUrl!,
            "payments",
            command,
            cancellationToken);

        return Unit.Value;
    }
}