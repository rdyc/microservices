using Product.Contract.Commands;
using Product.Domain.Repositories;

namespace Product.Domain.Validators
{
    internal class CreateProductValidator : ProductValidator<CreateProductCommand>
    {
        public CreateProductValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            ValidateName();
            ValidateDescription();
        }
    }
}