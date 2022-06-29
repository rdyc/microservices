using Product.Contract.Commands;
using Product.Domain.Repositories;

namespace Product.Domain.Validators;

internal class DeleteProductValidator : ProductValidator<DeleteProductCommand>
{
    public DeleteProductValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        ValidateId();
    }
}