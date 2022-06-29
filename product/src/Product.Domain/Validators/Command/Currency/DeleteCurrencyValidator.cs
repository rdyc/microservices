using Product.Contract.Commands;
using Product.Domain.Repositories;

namespace Product.Domain.Validators;

internal class DeleteCurrencyValidator : CurrencyValidator<DeleteCurrencyCommand>
{
    public DeleteCurrencyValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        ValidateId();
    }
}