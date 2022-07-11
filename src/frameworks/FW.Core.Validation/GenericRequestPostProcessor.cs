using MediatR;
using MediatR.Pipeline;

namespace FW.Core.Validation;

public class GenericRequestPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        Console.WriteLine("- All Done");

        return Task.CompletedTask;
    }
}