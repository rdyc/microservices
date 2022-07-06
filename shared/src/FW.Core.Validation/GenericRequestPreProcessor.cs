using MediatR;
using MediatR.Pipeline;

namespace FW.Core.Validation;

public class GenericRequestPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : IRequest
{
    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        Console.WriteLine("- Starting Up");

        return Task.CompletedTask;
    }
}