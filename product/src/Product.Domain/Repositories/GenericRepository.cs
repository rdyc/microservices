using System.Linq;
using Microsoft.EntityFrameworkCore;
using Product.Domain.Persistence;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Repositories
{
    internal class GenericRepository
    {
        private readonly ProductContext context;

        public GenericRepository(ProductContext context)
        {
            this.context = context;
        }

        public IQueryable<ProductEntity> GetAll()
        {
            return context.Products.AsQueryable();
        }

        public void Add(ProductEntity entity)
        {
            context.Entry(entity).State = EntityState.Added;
        }

        public void Update(ProductEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(ProductEntity entity)
        {
            context.Entry(entity).State = EntityState.Deleted;
        }
    }
}