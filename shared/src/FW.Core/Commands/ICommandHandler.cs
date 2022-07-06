using MediatR;

namespace FW.Core.Commands;

public interface ICommandHandler<in T> : IRequestHandler<T>
    where T : ICommand
{
}