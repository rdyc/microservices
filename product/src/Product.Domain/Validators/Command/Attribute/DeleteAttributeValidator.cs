using Product.Contract.Commands;
using Product.Domain.Repositories;

namespace Product.Domain.Validators
{
    internal class DeleteAttributeValidator : AttributeValidator<DeleteAttributeCommand>
    {
        public DeleteAttributeValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            ValidateId();
        }
    }
}