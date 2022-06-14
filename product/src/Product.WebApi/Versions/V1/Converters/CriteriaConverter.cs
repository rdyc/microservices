using System;
using AutoMapper;
using Product.Contract.Query;
using Product.WebApi.Versions.V1.Models;
using Shared.Infrastructure.Request;

namespace Product.WebApi.Versions.V1.Converters
{
    internal class CriteriaConverter : IValueConverter<GetAllItemsRequest, CriteriaQuery<ItemField>>
    {
        public CriteriaQuery<ItemField> Convert(GetAllItemsRequest sourceMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(sourceMember.Find))
            {
                var criteria = new CriteriaQuery<ItemField>();

                if (sourceMember.FindBy.HasValue)
                {
                    criteria.AddValues(sourceMember.FindBy.Value, sourceMember.Find);
                }
                else
                {
                    if (Guid.TryParse(sourceMember.Find, out Guid id))
                    {
                        criteria.AddValues(ItemField.Id, id);
                    }
                    criteria.AddValues(ItemField.Name, sourceMember.Find);
                    criteria.AddValues(ItemField.Description, sourceMember.Find);
                }

                return criteria;
            }

            return null;
        }
    }
}