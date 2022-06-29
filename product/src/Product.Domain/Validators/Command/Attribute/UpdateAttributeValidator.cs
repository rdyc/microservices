using Product.Contract.Commands;
using Product.Domain.Repositories;

namespace Product.Domain.Validators;

internal class UpdateAttributeValidator : AttributeValidator<UpdateAttributeCommand>
{
    public UpdateAttributeValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        ValidateId();
        ValidateName(true);
        ValidateUnit(true);
    }
}