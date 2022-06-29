using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Product.Contract.Enums;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Repositories;

internal interface IReferenceRepository
{
    IQueryable<CurrencyReferenceEntity> GetAllCurrencies();
    IQueryable<AttributeReferenceEntity> GetAllAttributes();
    Task<CurrencyReferenceEntity> GetOrCreateCurrencyAsync(Guid id, string name, string code, string symbol, CancellationToken cancellationToken = default);
    Task<AttributeReferenceEntity> GetOrCreateAttributeAsync(Guid id, string name, AttributeType type, string unit, CancellationToken cancellationToken = default);
}