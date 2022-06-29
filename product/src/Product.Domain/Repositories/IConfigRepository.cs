using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Product.Contract.Enums;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Repositories;

internal interface IConfigRepository
{
    IQueryable<CurrencyEntity> GetAllCurrencies();
    Task<CurrencyEntity> GetCurrencyAsync(Guid id, CancellationToken cancellationToken = default);
    CurrencyEntity CreateCurrency(string name, string code, string symbol);
    void UpdateCurrency(CurrencyEntity entity, Action<CurrencyEntity> action);
    IQueryable<AttributeEntity> GetAllAttributes();
    Task<AttributeEntity> GetAttributeAsync(Guid id, CancellationToken cancellationToken = default);
    AttributeEntity CreateAttribute(string name, AttributeType type, string unit);
    void UpdateAttribute(AttributeEntity entity, Action<AttributeEntity> action);
}