using Product.Contract.Command;
using Product.Domain.Repository;

namespace Product.Domain.Validator.Command
{
    internal class DeleteItemValidator : ItemValidator<DeleteItemCommand>
    {
        public DeleteItemValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            ValidateId();
        }
    }
}