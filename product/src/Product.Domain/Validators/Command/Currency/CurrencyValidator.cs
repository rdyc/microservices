// This source code (ProductValidator.cs) is Copyright © PT. Xsis Mitra Utama.
// You MAY NOT copied, reproduced, published, distributed or transmitted
// to or stored in any manner without priorwritten consent from PT. Xsis Mitra Utama.

using FluentValidation;
using Product.Contract.Commands;
using Product.Domain.Repositories;

namespace Product.Domain.Validators
{
    internal class CurrencyValidator<T> : AbstractValidator<T>
        where T : CurrencyCommand
    {
        private readonly IUnitOfWork unitOfWork;

        public CurrencyValidator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        protected void ValidateId()
        {
            RuleFor(c => c.Id)
                .NotNull()
                .MustBeExistCurrencyAsync(unitOfWork.Config).WithMessage("The resource was not found");
        }

        protected void ValidateName(bool isUpdate = false)
        {
            var rule = RuleFor(c => c.Name).NotEmpty();

            if (!isUpdate)
            {
                rule.MustBeUniqueCurrencyNameAsync(unitOfWork.Config).WithMessage("The name already exist");
            }
            else
            {
                rule.MustBeUniqueCurrencyNameIdAsync(unitOfWork.Config).WithMessage("The name already taken");
            }
        }

        protected void ValidateCode(bool isUpdate = false)
        {
            var rule = RuleFor(c => c.Code).NotEmpty();

            if (!isUpdate)
            {
                rule.MustBeUniqueCurrencyCodeAsync(unitOfWork.Config).WithMessage("The code already exist");
            }
            else
            {
                rule.MustBeUniqueCurrencyCodeIdAsync(unitOfWork.Config).WithMessage("The code already taken");
            }
        }

        protected void ValidateSymbol(bool isUpdate = false)
        {
            var rule = RuleFor(c => c.Symbol).NotEmpty();

            if (!isUpdate)
            {
                rule.MustBeUniqueCurrencySymbolAsync(unitOfWork.Config).WithMessage("The symbol already exist");
            }
            else
            {
                rule.MustBeUniqueCurrencySymbolIdAsync(unitOfWork.Config).WithMessage("The symbol already taken");
            }
        }
    }
}