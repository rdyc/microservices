// This source code (ClientValidatorExtension.cs) is Copyright © PT. Xsis Mitra Utama.
// You MAY NOT copied, reproduced, published, distributed or transmitted
// to or stored in any manner without priorwritten consent from PT. Xsis Mitra Utama.

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Commands;
using Product.Domain.Repositories;
using System;

namespace Product.Domain.Validator.Command
{
    internal static class ProductValidatorExtension
    {
        internal static IRuleBuilderOptions<T, Guid?> MustBeExistAsync<T>(this IRuleBuilder<T, Guid?> ruleBuilder, IProductRepository productRepository)
            where T : ProductCommand
        {
            return ruleBuilder.MustAsync(async (context, id, cancellationToken) =>
            {
                return await productRepository.GetAll().AnyAsync(e => e.Id.Equals(id.Value), cancellationToken);
            });
        }

        public static IRuleBuilderOptions<T, string> MustBeUniqueNameAsync<T>(this IRuleBuilder<T, string> ruleBuilder, IProductRepository productRepository)
            where T : ProductCommand
        {
            return ruleBuilder.MustAsync(async (context, value, cancellationToken) =>
            {
                return !await productRepository.GetAll().AnyAsync(e => e.Name.Equals(value));
            });
        }

        public static IRuleBuilderOptions<T, string> MustBeUniqueNameIdAsync<T>(this IRuleBuilder<T, string> ruleBuilder, IProductRepository productRepository)
            where T : ProductCommand
        {
            return ruleBuilder.MustAsync(async (context, value, cancellationToken) =>
            {
                return !await productRepository.GetAll().AnyAsync(e => !e.Id.Equals(context.Id.Value) && e.Name.Equals(value));
            });
        }
    }
}