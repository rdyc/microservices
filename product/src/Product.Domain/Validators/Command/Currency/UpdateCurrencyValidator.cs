using Product.Contract.Commands;
using Product.Domain.Repositories;

namespace Product.Domain.Validators
{
    internal class UpdateCurrencyValidator : CurrencyValidator<UpdateCurrencyCommand>
    {
        public UpdateCurrencyValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            ValidateId();
            ValidateName(true);
            ValidateCode(true);
            ValidateSymbol(true);
        }
    }
}