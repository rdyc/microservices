// This source code (ClientValidatorExtension.cs) is Copyright © PT. Xsis Mitra Utama.
// You MAY NOT copied, reproduced, published, distributed or transmitted
// to or stored in any manner without priorwritten consent from PT. Xsis Mitra Utama.

using System;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Commands;
using Product.Domain.Repositories;

namespace Product.Domain.Validators;

internal static class AttributeValidatorExtension
{
    internal static IRuleBuilderOptions<T, Guid> MustBeExistAttributeAsync<T>(this IRuleBuilder<T, Guid> ruleBuilder, IConfigRepository configRepository)
    {
        return ruleBuilder.MustAsync(async (id, cancellationToken) =>
        {
            return await configRepository.GetAllAttributes().AnyAsync(e => e.Id.Equals(id), cancellationToken);
        });
    }

    public static IRuleBuilderOptions<T, string> MustBeUniqueAttributeNameAsync<T>(this IRuleBuilder<T, string> ruleBuilder, IConfigRepository configRepository)
        where T : AttributeCommand
    {
        return ruleBuilder.MustAsync(async (context, value, cancellationToken) =>
        {
            return !await configRepository.GetAllAttributes().AnyAsync(e => e.Name.Equals(value), cancellationToken);
        });
    }

    public static IRuleBuilderOptions<T, string> MustBeUniqueAttributeNameIdAsync<T>(this IRuleBuilder<T, string> ruleBuilder, IConfigRepository configRepository)
        where T : AttributeCommand
    {
        return ruleBuilder.MustAsync(async (context, value, cancellationToken) =>
        {
            return !await configRepository.GetAllAttributes().AnyAsync(e => !e.Id.Equals(context.Id.Value) && e.Name.Equals(value), cancellationToken);
        });
    }

    public static IRuleBuilderOptions<T, string> MustBeUniqueAttributeUnitAsync<T>(this IRuleBuilder<T, string> ruleBuilder, IConfigRepository configRepository)
        where T : AttributeCommand
    {
        return ruleBuilder.MustAsync(async (context, value, cancellationToken) =>
        {
            return !await configRepository.GetAllAttributes().AnyAsync(e => e.Unit.Equals(value), cancellationToken);
        });
    }

    public static IRuleBuilderOptions<T, string> MustBeUniqueAttributeUnitIdAsync<T>(this IRuleBuilder<T, string> ruleBuilder, IConfigRepository configRepository)
        where T : AttributeCommand
    {
        return ruleBuilder.MustAsync(async (context, value, cancellationToken) =>
        {
            return !await configRepository.GetAllAttributes().AnyAsync(e => !e.Id.Equals(context.Id.Value) && e.Unit.Equals(value), cancellationToken);
        });
    }
}