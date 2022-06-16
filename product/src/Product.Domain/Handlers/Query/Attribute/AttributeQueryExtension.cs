using System;
using System.Linq;
using Product.Contract.Queries;
using Product.Domain.Persistence.Entities;
using Shared.Infrastructure.Enums;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Request;

namespace Product.Domain.Handlers
{
    internal static class AttributeQueryExtension
    {
        public static IQueryable<AttributeEntity> WithCriteria(this IQueryable<AttributeEntity> attributes, CriteriaQuery<AttributeField> criteriaQuery = null)
        {
            if (criteriaQuery == null)
                return attributes;

            var predicate = PredicateBuilder.False<AttributeEntity>();

            foreach (var criteria in criteriaQuery.Values)
            {
                switch (criteria.Field)
                {
                    case AttributeField.Id:
                        predicate = predicate.Or(e => e.Id.Equals((Guid)criteria.Value));
                        break;
                    case AttributeField.Name:
                        predicate = predicate.Or(e => e.Name.Contains(criteria.Value.ToString()));
                        break;
                    case AttributeField.Type:
                        predicate = predicate.Or(e => e.Type.Equals(criteria.Value.ToString()));
                        break;
                    case AttributeField.Unit:
                        predicate = predicate.Or(e => e.Unit.Contains(criteria.Value.ToString()));
                        break;
                    case AttributeField.Value:
                        predicate = predicate.Or(e => e.Type.Equals(criteria.Value.ToString()));
                        break;
                }
            }

            return attributes.Where(predicate);
        }

        public static IQueryable<AttributeEntity> WithOrdered(this IQueryable<AttributeEntity> attributes, OrderedQuery<AttributeField> orderedQuery = null)
        {
            if (orderedQuery == null)
                return attributes;

            switch (orderedQuery.Direction)
            {
                case Direction.Ascending:
                    attributes = attributes.OrderBy(orderedQuery.Field.ToString());
                    break;

                case Direction.Descending:
                    attributes = attributes.OrderByDescending(orderedQuery.Field.ToString());
                    break;
            }

            return attributes;
        }

        public static IQueryable<AttributeEntity> WithPaged(this IQueryable<AttributeEntity> attributes, PagedQuery pagedQuery = null)
        {
            if (pagedQuery == null)
                return attributes;

            return attributes.Skip(pagedQuery.Index * pagedQuery.Size).Take(pagedQuery.Size);
        }
    }
}