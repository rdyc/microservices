using FW.Core.Commands;
using FW.Core.Requests;
using MediatR;

namespace Order.Payments.DiscardingPayment;

public record DiscardPayment(
    Guid PaymentId,
    DiscardReason DiscardReason
) : ICommand
{
    public static DiscardPayment Create(Guid paymentId)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));

        return new(paymentId, DiscardReason.OrderCancelled);
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
        await commandBus.Post(
            config.PaymentsUrl!,
            $"payments/{command.PaymentId}/discard",
            command,
            cancellationToken);

        return Unit.Value;
    }
}