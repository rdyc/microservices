using Product.Contract.Commands;
using Product.Domain.Repositories;

namespace Product.Domain.Validators;

internal class CreateCurrencyValidator : CurrencyValidator<CreateCurrencyCommand>
{
    public CreateCurrencyValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        ValidateName();
        ValidateCode();
        ValidateSymbol();
    }
}