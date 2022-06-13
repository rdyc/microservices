using System;
using System.Linq;
using Product.Contract.Query;
using Product.Domain.Entity;
using Shared.Infrastructure.Enums;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Request;

namespace Product.Domain.Handler.Query
{
    internal static class ItemQueryExtension
    {
        internal static IQueryable<ItemEntity> WithCriteria(this IQueryable<ItemEntity> items, CriteriaQuery<ItemField> criteriaQuery = null)
        {
            if (criteriaQuery == null)
                return items;

            var predicate = PredicateBuilder.False<ItemEntity>();

            foreach (var criteria in criteriaQuery.Values)
            {
                switch (criteria.Field)
                {
                    case ItemField.Id:
                        predicate = predicate.Or(e => e.Id.Equals((Guid)criteria.Value));
                        break;
                    case ItemField.Name:
                        predicate = predicate.Or(e => e.Name.Contains(criteria.Value.ToString()));
                        break;
                    case ItemField.Description:
                        predicate = predicate.Or(e => e.Description.Contains(criteria.Value.ToString()));
                        break;
                }
            }

            return items.Where(predicate);
        }

        internal static IQueryable<ItemEntity> WithOrdered(this IQueryable<ItemEntity> items, OrderedQuery<ItemField> orderedQuery = null)
        {
            if (orderedQuery == null)
                return items;

            switch (orderedQuery.Direction)
            {
                case Direction.Ascending:
                    items = items.OrderBy(orderedQuery.Field.ToString());
                    break;

                case Direction.Descending:
                    items = items.OrderByDescending(orderedQuery.Field.ToString());
                    break;
            }

            return items;
        }

        internal static IQueryable<ItemEntity> WithPaged(this IQueryable<ItemEntity> items, PagedQuery pagedQuery = null)
        {
            if (pagedQuery == null)
                return items;

            return items.Skip(pagedQuery.Index * pagedQuery.Size).Take(pagedQuery.Size);
        }
    }
}