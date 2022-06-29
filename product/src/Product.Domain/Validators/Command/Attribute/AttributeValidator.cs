// This source code (ProductValidator.cs) is Copyright © PT. Xsis Mitra Utama.
// You MAY NOT copied, reproduced, published, distributed or transmitted
// to or stored in any manner without priorwritten consent from PT. Xsis Mitra Utama.

using FluentValidation;
using Product.Contract.Commands;
using Product.Domain.Repositories;

namespace Product.Domain.Validators;

internal class AttributeValidator<T> : AbstractValidator<T>
    where T : AttributeCommand
{
    private readonly IUnitOfWork unitOfWork;

    public AttributeValidator(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    protected void ValidateId()
    {
        Transform(f => f.Id, t => t.Value)
            .MustBeExistAttributeAsync(unitOfWork.Config)
            .WithMessage("The requested attribute was not found");
    }

    protected void ValidateName(bool isUpdate = false)
    {
        var rule = RuleFor(c => c.Name).NotEmpty();

        if (!isUpdate)
        {
            rule.MustBeUniqueAttributeNameAsync(unitOfWork.Config)
                .WithMessage("The attribute name already exist");
        }
        else
        {
            rule.MustBeUniqueAttributeNameIdAsync(unitOfWork.Config)
                .WithMessage("The attribute name already taken");
        }
    }

    protected void ValidateUnit(bool isUpdate = false)
    {
        var rule = RuleFor(c => c.Unit).NotEmpty();

        if (!isUpdate)
        {
            rule.MustBeUniqueAttributeUnitAsync(unitOfWork.Config)
                .WithMessage("The attribute unit already exist");
        }
        else
        {
            rule.MustBeUniqueAttributeUnitIdAsync(unitOfWork.Config)
                .WithMessage("The attribute unit already taken");
        }
    }
}