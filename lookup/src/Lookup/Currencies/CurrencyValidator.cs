using FluentValidation;

namespace Lookup.Currencies;

public class CurrencyValidator<T> : AbstractValidator<T>
    where T : CurrencyCommand
{
    protected void ValidateId()
    {
        RuleFor(p => p.Id).NotEmpty();
    }

    protected void ValidateName()
    {
        RuleFor(p => p.Name).NotEmpty();
    }

    protected void ValidateCode()
    {
        RuleFor(p => p.Code).NotEmpty().MaximumLength(3);
    }

    protected void ValidateSymbol()
    {
        RuleFor(p => p.Symbol).NotEmpty().MaximumLength(3);
    }
}