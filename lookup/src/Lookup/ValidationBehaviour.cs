using FluentValidation;
using MediatR;
using MediatR.Pipeline;

namespace Lookup;

internal class GenericRequestPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : IRequest
{
    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        Console.WriteLine("- Starting Up");

        return Task.CompletedTask;
    }
}

internal class GenericRequestPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        Console.WriteLine("- All Done");

        return Task.CompletedTask;
    }
}

internal class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> validator;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validator)
    {
        this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        if (validator.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(validator.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}