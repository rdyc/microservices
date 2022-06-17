// This source code (ClientValidatorExtension.cs) is Copyright © PT. Xsis Mitra Utama.
// You MAY NOT copied, reproduced, published, distributed or transmitted
// to or stored in any manner without priorwritten consent from PT. Xsis Mitra Utama.

using System;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Commands;
using Product.Domain.Repositories;

namespace Product.Domain.Validators
{
    internal static class CurrencyValidatorExtension
    {
        internal static IRuleBuilderOptions<T, Guid> MustBeExistCurrencyAsync<T>(this IRuleBuilder<T, Guid> ruleBuilder, IConfigRepository configRepository)
        {
            return ruleBuilder.MustAsync(async (id, cancellationToken) =>
            {
                return await configRepository.GetAllCurrencies().AnyAsync(e => e.Id.Equals(id), cancellationToken);
            });
        }

        public static IRuleBuilderOptions<T, string> MustBeUniqueCurrencyNameAsync<T>(this IRuleBuilder<T, string> ruleBuilder, IConfigRepository configRepository)
            where T : CurrencyCommand
        {
            return ruleBuilder.MustAsync(async (context, value, cancellationToken) =>
            {
                return !await configRepository.GetAllCurrencies().AnyAsync(e => e.Name.Equals(value));
            });
        }

        public static IRuleBuilderOptions<T, string> MustBeUniqueCurrencyNameIdAsync<T>(this IRuleBuilder<T, string> ruleBuilder, IConfigRepository configRepository)
            where T : CurrencyCommand
        {
            return ruleBuilder.MustAsync(async (context, value, cancellationToken) =>
            {
                return !await configRepository.GetAllCurrencies().AnyAsync(e => !e.Id.Equals(context.Id.Value) && e.Name.Equals(value));
            });
        }

        public static IRuleBuilderOptions<T, string> MustBeUniqueCurrencyCodeAsync<T>(this IRuleBuilder<T, string> ruleBuilder, IConfigRepository configRepository)
            where T : CurrencyCommand
        {
            return ruleBuilder.MustAsync(async (context, value, cancellationToken) =>
            {
                return !await configRepository.GetAllCurrencies().AnyAsync(e => e.Code.Equals(value));
            });
        }

        public static IRuleBuilderOptions<T, string> MustBeUniqueCurrencyCodeIdAsync<T>(this IRuleBuilder<T, string> ruleBuilder, IConfigRepository configRepository)
            where T : CurrencyCommand
        {
            return ruleBuilder.MustAsync(async (context, value, cancellationToken) =>
            {
                return !await configRepository.GetAllCurrencies().AnyAsync(e => !e.Id.Equals(context.Id.Value) && e.Code.Equals(value));
            });
        }

        public static IRuleBuilderOptions<T, string> MustBeUniqueCurrencySymbolAsync<T>(this IRuleBuilder<T, string> ruleBuilder, IConfigRepository configRepository)
            where T : CurrencyCommand
        {
            return ruleBuilder.MustAsync(async (context, value, cancellationToken) =>
            {
                return !await configRepository.GetAllCurrencies().AnyAsync(e => e.Symbol.Equals(value));
            });
        }

        public static IRuleBuilderOptions<T, string> MustBeUniqueCurrencySymbolIdAsync<T>(this IRuleBuilder<T, string> ruleBuilder, IConfigRepository configRepository)
            where T : CurrencyCommand
        {
            return ruleBuilder.MustAsync(async (context, value, cancellationToken) =>
            {
                return !await configRepository.GetAllCurrencies().AnyAsync(e => !e.Id.Equals(context.Id.Value) && e.Symbol.Equals(value));
            });
        }
    }
}