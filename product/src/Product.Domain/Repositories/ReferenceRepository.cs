using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Enums;
using Product.Domain.Persistence;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Repositories;

internal class ReferenceRepository : IReferenceRepository
{
    private readonly ProductContext context;

    public ReferenceRepository(ProductContext context)
    {
        this.context = context;
    }

    public IQueryable<AttributeReferenceEntity> GetAllAttributes()
    {
        return context.AttributeReferences.AsQueryable();
    }

    public IQueryable<CurrencyReferenceEntity> GetAllCurrencies()
    {
        return context.CurrencyReferences.AsQueryable();
    }

    public async Task<CurrencyReferenceEntity> GetOrCreateCurrencyAsync(Guid id, string name, string code, string symbol, CancellationToken cancellationToken = default)
    {
        var currency = await context.CurrencyReferences
            .SingleOrDefaultAsync(e =>
                e.Id.Equals(id) &&
                e.Name.Equals(name) &&
                e.Code.Equals(code) &&
                e.Symbol.Equals(symbol),
            cancellationToken);

        if (currency == null)
        {
            currency = new CurrencyReferenceEntity(id, name, code, symbol);
            context.Entry(currency).State = EntityState.Added;
        }

        return currency;
    }

    public async Task<AttributeReferenceEntity> GetOrCreateAttributeAsync(Guid id, string name, AttributeType type, string unit, CancellationToken cancellationToken = default)
    {
        var attribute = await context.AttributeReferences
            .SingleOrDefaultAsync(e =>
                e.Id.Equals(id) &&
                e.Name.Equals(name) &&
                e.Type.Equals(type) &&
                e.Unit.Equals(unit),
            cancellationToken);

        if (attribute == null)
        {
            attribute = new AttributeReferenceEntity(id, name, type, unit);
            context.Entry(attribute).State = EntityState.Added;
        }

        return attribute;
    }
}