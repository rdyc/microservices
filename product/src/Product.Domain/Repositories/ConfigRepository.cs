using System.Linq;
using Microsoft.EntityFrameworkCore;
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

        public IQueryable<AttributeEntity> GetAllAttributes()
        {
            return context.Attributes.AsQueryable();
        }

        public IQueryable<CurrencyEntity> GetAllCurrencies()
        {
            return context.Currencies.AsQueryable();
        }

        public void Add(AttributeEntity entity)
        {
            context.Entry(entity).State = EntityState.Added;
        }

        public void Update(AttributeEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(AttributeEntity entity)
        {
            context.Entry(entity).State = EntityState.Deleted;
        }

        public void Add(CurrencyEntity entity)
        {
            context.Entry(entity).State = EntityState.Added;
        }

        public void Update(CurrencyEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(CurrencyEntity entity)
        {
            context.Entry(entity).State = EntityState.Deleted;
        }
    }
}