// This source code (ItemValidator.cs) is Copyright © PT. Xsis Mitra Utama.
// You MAY NOT copied, reproduced, published, distributed or transmitted
// to or stored in any manner without priorwritten consent from PT. Xsis Mitra Utama.

using FluentValidation;
using Product.Contract.Command;
using Product.Domain.Repository;

namespace Product.Domain.Validator.Command
{
    internal class ItemValidator<T> : AbstractValidator<T>
        where T : ItemCommand
    {
        private readonly IUnitOfWork unitOfWork;

        public ItemValidator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        protected void ValidateId()
        {
            RuleFor(c => c.Id)
                .NotNull()
                .MustBeExistAsync(unitOfWork.Item).WithMessage("The resource was not found");
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
                rule.MustBeUniqueNameAsync(unitOfWork.Item).WithMessage("The name already exist");
            }
            else
            {
                rule.MustBeUniqueNameIdAsync(unitOfWork.Item).WithMessage("The name already taken");
            }
        }
    }
}