using Product.Contract.Command;
using Product.Domain.Repository;

namespace Product.Domain.Validator.Command
{
    internal class UpdateItemValidator : ItemValidator<UpdateItemCommand>
    {
        public UpdateItemValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            ValidateId();
            ValidateName(true);
            ValidateDescription();
        }
    }
}