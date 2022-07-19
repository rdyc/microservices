using MediatR;

namespace FW.Core.Commands;

public class CommandBus : ICommandBus
{
    private readonly IMediator mediator;

    public CommandBus(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
    {
        return mediator.Send(command, cancellationToken);
    }
}