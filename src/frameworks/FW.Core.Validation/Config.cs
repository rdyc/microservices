using FW.Core.Commands;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FW.Core.Validation;

public static class Config
{
    public static IServiceCollection AddCommandValidator<TCommand, TValidatorHandler>(
        this IServiceCollection services
    )
        where TCommand : ICommand
        where TValidatorHandler : AbstractValidator<TCommand>
    {
        return services.AddSingleton<IValidator<TCommand>, TValidatorHandler>();
    }
}