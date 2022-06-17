using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Product.Domain.Events;
using Product.Domain.Persistence;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Repositories
{
    internal class ProductRepository : IProductRepository
    {
        private readonly ProductContext context;

        public ProductRepository(ProductContext context)
        {
            this.context = context;
        }

        public IQueryable<ProductEntity> GetAll()
        {
            return context.Products.AsQueryable();
        }

        public async Task<ProductEntity> GetDetailAsync(Guid productId, CancellationToken cancellationToken)
        {
            return await context.Products
                .Include(e => e.Currency)
                .Include(e => e.Attributes)
                    .ThenInclude(e => e.Attribute)
                .SingleOrDefaultAsync(e => e.Id.Equals(productId));
        }

        public ProductEntity Create(string name, string description, CurrencyReferenceEntity currency, decimal price)
        {
            var product = new ProductEntity(name, description, currency, price);
            product.Events.Add(new ProductCreatedEvent(product));
            context.Entry(product).State = EntityState.Added;
            return product;
        }

        public void Update(ProductEntity product, Action<ProductEntity> action)
        {
            action.Invoke(product);
            context.Entry(product).State = EntityState.Modified;
        }
    }
}