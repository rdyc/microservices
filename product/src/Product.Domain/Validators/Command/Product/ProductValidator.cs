// This source code (ProductValidator.cs) is Copyright © PT. Xsis Mitra Utama.
// You MAY NOT copied, reproduced, published, distributed or transmitted
// to or stored in any manner without priorwritten consent from PT. Xsis Mitra Utama.

using FluentValidation;
using Product.Contract.Commands;
using Product.Domain.Repositories;

namespace Product.Domain.Validators
{
    internal class ProductValidator<T> : AbstractValidator<T>
        where T : ProductCommand
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductValidator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        protected void ValidateId()
        {
            RuleFor(c => c.Id)
                .NotNull()
                .MustBeExistProductAsync(unitOfWork.Product).WithMessage("The resource was not found");
        }

        protected void ValidateDescription()
        {
            RuleFor(c => c.Description).NotEmpty();
        }

        protected void ValidateName(bool isUpdate = false)
        {
            var rule = RuleFor(c => c.Name).NotEmpty();

            if (!isUpdate)
            {
                rule.MustBeUniqueProductNameAsync(unitOfWork.Product).WithMessage("The name already exist");
            }
            else
            {
                rule.MustBeUniqueProductNameIdAsync(unitOfWork.Product).WithMessage("The name already taken");
            }
        }
    }
}