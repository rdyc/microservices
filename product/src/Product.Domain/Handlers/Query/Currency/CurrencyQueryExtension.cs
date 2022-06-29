using System;
using System.Linq;
using Product.Contract.Queries;
using Product.Domain.Persistence.Entities;
using Shared.Infrastructure.Enums;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Request;

namespace Product.Domain.Handlers;

internal static class CurrencyQueryExtension
{
    public static IQueryable<CurrencyEntity> WithCriteria(this IQueryable<CurrencyEntity> currencies, CriteriaQuery<CurrencyField> criteriaQuery = null)
    {
        if (criteriaQuery == null)
            return currencies;

        var predicate = PredicateBuilder.False<CurrencyEntity>();

        foreach (var criteria in criteriaQuery.Values)
        {
            switch (criteria.Field)
            {
                case CurrencyField.Id:
                    predicate = predicate.Or(e => e.Id.Equals((Guid)criteria.Value));
                    break;
                case CurrencyField.Name:
                    predicate = predicate.Or(e => e.Name.Contains(criteria.Value.ToString()));
                    break;
                case CurrencyField.Code:
                    predicate = predicate.Or(e => e.Code.Contains(criteria.Value.ToString()));
                    break;
                case CurrencyField.Symbol:
                    predicate = predicate.Or(e => e.Symbol.Contains(criteria.Value.ToString()));
                    break;
            }
        }

        return currencies.Where(predicate);
    }

    public static IQueryable<CurrencyEntity> WithOrdered(this IQueryable<CurrencyEntity> currencies, OrderedQuery<CurrencyField> orderedQuery = null)
    {
        if (orderedQuery == null)
            return currencies;

        switch (orderedQuery.Direction)
        {
            case Direction.Ascending:
                currencies = currencies.OrderBy(orderedQuery.Field.ToString());
                break;

            case Direction.Descending:
                currencies = currencies.OrderByDescending(orderedQuery.Field.ToString());
                break;
        }

        return currencies;
    }

    public static IQueryable<CurrencyEntity> WithPaged(this IQueryable<CurrencyEntity> currencies, PagedQuery pagedQuery = null)
    {
        if (pagedQuery == null)
            return currencies;

        return currencies.Skip(pagedQuery.Index * pagedQuery.Size).Take(pagedQuery.Size);
    }
}