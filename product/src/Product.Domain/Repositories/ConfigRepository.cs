using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Enums;
using Product.Domain.Persistence;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Repositories
{
    internal class ConfigRepository : IConfigRepository
    {
        private readonly ProductContext context;

        public ConfigRepository(ProductContext context)
        {
            this.context = context;
        }

        public IQueryable<CurrencyEntity> GetAllCurrencies()
        {
            return context.Currencies.AsQueryable();
        }

        public async Task<CurrencyEntity> GetCurrencyAsync(Guid id, CancellationToken cancellationToken)
        {
            return await GetAllCurrencies().SingleAsync(e => e.Id.Equals(id), cancellationToken);
        }

        public CurrencyEntity CreateCurrency(string name, string code, string symbol)
        {
            var entity = new CurrencyEntity(name, code, symbol);
            context.Entry(entity).State = EntityState.Added;
            return entity;
        }

        public void UpdateCurrency(CurrencyEntity entity, Action<CurrencyEntity> action)
        {
            action.Invoke(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public IQueryable<AttributeEntity> GetAllAttributes()
        {
            return context.Attributes.AsQueryable();
        }

        public async Task<AttributeEntity> GetAttributeAsync(Guid id, CancellationToken cancellationToken)
        {
            return await GetAllAttributes().SingleAsync(e => e.Id.Equals(id), cancellationToken);
        }

        public AttributeEntity CreateAttribute(string name, AttributeType type, string unit)
        {
            var entity = new AttributeEntity(name, type, unit);
            context.Entry(entity).State = EntityState.Added;
            return entity;
        }

        public void UpdateAttribute(AttributeEntity entity, Action<AttributeEntity> action)
        {
            action.Invoke(entity);
            context.Entry(entity).State = EntityState.Modified;
        }
    }
}