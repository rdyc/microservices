using Product.Contract.Commands;
using Product.Domain.Repositories;

namespace Product.Domain.Validator.Command
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