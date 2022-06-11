using Product.Contract.Command;
using Product.Domain.Repository;

namespace Product.Domain.Validator.Command
{
    internal class CreateItemValidator : ItemValidator<CreateItemCommand>
    {
        public CreateItemValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            ValidateName();
            ValidateDescription();
        }
    }
}