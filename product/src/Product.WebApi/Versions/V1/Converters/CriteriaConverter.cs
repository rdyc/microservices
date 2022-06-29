using System;
using AutoMapper;
using Product.Contract.Queries;
using Product.WebApi.Versions.V1.Models;
using Shared.Infrastructure.Request;

namespace Product.WebApi.Versions.V1.Converters;

internal class CriteriaConverter :
    IValueConverter<GetAllAttributesRequest, CriteriaQuery<AttributeField>>,
    IValueConverter<GetAllCurrenciesRequest, CriteriaQuery<CurrencyField>>,
    IValueConverter<GetAllProductsRequest, CriteriaQuery<ProductField>>
{
    public CriteriaQuery<AttributeField> Convert(GetAllAttributesRequest sourceMember, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(sourceMember.Find))
        {
            var criteria = new CriteriaQuery<AttributeField>();

            if (sourceMember.FindBy.HasValue)
            {
                criteria.AddValues(sourceMember.FindBy.Value, sourceMember.Find);
            }
            else
            {
                if (Guid.TryParse(sourceMember.Find, out Guid id))
                {
                    criteria.AddValues(AttributeField.Id, id);
                }
                criteria.AddValues(AttributeField.Name, sourceMember.Find);
                criteria.AddValues(AttributeField.Type, sourceMember.Find);
                criteria.AddValues(AttributeField.Unit, sourceMember.Find);
                criteria.AddValues(AttributeField.Value, sourceMember.Find);
            }

            return criteria;
        }

        return null;
    }

    public CriteriaQuery<CurrencyField> Convert(GetAllCurrenciesRequest sourceMember, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(sourceMember.Find))
        {
            var criteria = new CriteriaQuery<CurrencyField>();

            if (sourceMember.FindBy.HasValue)
            {
                criteria.AddValues(sourceMember.FindBy.Value, sourceMember.Find);
            }
            else
            {
                if (Guid.TryParse(sourceMember.Find, out Guid id))
                {
                    criteria.AddValues(CurrencyField.Id, id);
                }
                criteria.AddValues(CurrencyField.Name, sourceMember.Find);
                criteria.AddValues(CurrencyField.Code, sourceMember.Find);
                criteria.AddValues(CurrencyField.Symbol, sourceMember.Find);
            }

            return criteria;
        }

        return null;
    }

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