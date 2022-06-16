using System.Linq;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Repositories
{
    internal interface IConfigRepository
    {
        IQueryable<AttributeEntity> GetAllAttributes();
        IQueryable<CurrencyEntity> GetAllCurrencies();
        void Add(AttributeEntity entity);
        void Update(AttributeEntity entity);
        void Delete(AttributeEntity entity);
        void Add(CurrencyEntity entity);
        void Update(CurrencyEntity entity);
        void Delete(CurrencyEntity entity);
    }
}