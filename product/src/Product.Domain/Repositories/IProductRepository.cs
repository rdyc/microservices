using System.Linq;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Repositories
{
    internal interface IProductRepository
    {
        IQueryable<ProductEntity> GetAll();
        void Add(ProductEntity entity);
        void Update(ProductEntity entity);
        void Delete(ProductEntity entity);
    }
}