using Product.Contract.Commands;
using Product.Domain.Repositories;

namespace Product.Domain.Validators
{
    internal class CreateAttributeValidator : AttributeValidator<CreateAttributeCommand>
    {
        public CreateAttributeValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            ValidateName();
            ValidateUnit();
        }
    }
}