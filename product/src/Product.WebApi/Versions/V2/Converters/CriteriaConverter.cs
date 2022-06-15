using System;
using AutoMapper;
using Product.Contract.Queries;
using Product.WebApi.Versions.V2.Models;
using Shared.Infrastructure.Request;

namespace Product.WebApi.Versions.V2.Converters
{
    internal class CriteriaConverter : IValueConverter<GetAllProductsRequest, CriteriaQuery<ProductField>>
    {
        public CriteriaQuery<ProductField> Convert(GetAllProductsRequest sourceMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(sourceMember.Find))
            {
                var criteria = new CriteriaQuery<ProductField>();

                if (sourceMember.FindBy.HasValue)
                {
                    criteria.AddValues(sourceMember.FindBy.Value, sourceMember.Find);
                }
                else
                {
                    if (Guid.TryParse(sourceMember.Find, out Guid id))
                    {
                        criteria.AddValues(ProductField.Id, id);
                    }
                    criteria.AddValues(ProductField.Name, sourceMember.Find);
                    criteria.AddValues(ProductField.Description, sourceMember.Find);
                }

                return criteria;
            }

            return null;
        }
    }
}