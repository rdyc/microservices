using System;
using System.Linq;
using Product.Contract.Queries;
using Product.Domain.Persistence.Entities;
using Shared.Infrastructure.Enums;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Request;

namespace Product.Domain.Handlers;

internal static class ProductQueryExtension
{
    public static IQueryable<ProductEntity> WithCriteria(this IQueryable<ProductEntity> products, CriteriaQuery<ProductField> criteriaQuery = null)
    {
        if (criteriaQuery == null)
            return products;

        var predicate = PredicateBuilder.False<ProductEntity>();

        foreach (var criteria in criteriaQuery.Values)
        {
            switch (criteria.Field)
            {
                case ProductField.Id:
                    predicate = predicate.Or(e => e.Id.Equals((Guid)criteria.Value));
                    break;
                case ProductField.Name:
                    predicate = predicate.Or(e => e.Name.Contains(criteria.Value.ToString()));
                    break;
                case ProductField.Description:
                    predicate = predicate.Or(e => e.Description.Contains(criteria.Value.ToString()));
                    break;
            }
        }

        return products.Where(predicate);
    }

    public static IQueryable<ProductEntity> WithOrdered(this IQueryable<ProductEntity> products, OrderedQuery<ProductField> orderedQuery = null)
    {
        if (orderedQuery == null)
            return products;

        switch (orderedQuery.Direction)
        {
            case Direction.Ascending:
                products = products.OrderBy(orderedQuery.Field.ToString());
                break;

            case Direction.Descending:
                products = products.OrderByDescending(orderedQuery.Field.ToString());
                break;
        }

        return products;
    }

    public static IQueryable<ProductEntity> WithPaged(this IQueryable<ProductEntity> products, PagedQuery pagedQuery = null)
    {
        if (pagedQuery == null)
            return products;

        return products.Skip(pagedQuery.Index * pagedQuery.Size).Take(pagedQuery.Size);
    }
}