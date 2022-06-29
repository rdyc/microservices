using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Repositories;

internal interface IProductRepository
{
    IQueryable<ProductEntity> GetAll();
    Task<ProductEntity> GetDetailAsync(Guid productId, CancellationToken cancellationToken);
    ProductEntity Create(string name, string description, CurrencyReferenceEntity currency, decimal price);
    void Update(ProductEntity product, Action<ProductEntity> action);
}