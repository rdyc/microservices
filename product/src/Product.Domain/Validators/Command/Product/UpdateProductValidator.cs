using Product.Contract.Commands;
using Product.Domain.Repositories;

namespace Product.Domain.Validators
{
    internal class UpdateProductValidator : ProductValidator<UpdateProductCommand>
    {
        public UpdateProductValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            ValidateId();
            ValidateName(true);
            ValidateDescription();
            ValidateCurrency();
            ValidatePrice();
        }
    }
}