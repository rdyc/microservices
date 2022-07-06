using MediatR;

namespace FW.Core.Queries;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}