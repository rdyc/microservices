using System.Linq;
using Microsoft.EntityFrameworkCore;
using Product.Domain.Context;
using Product.Domain.Entity;

namespace Product.Domain.Repository
{
    internal class ItemRepository : IItemRepository
    {
        private readonly ProductContext context;

        public ItemRepository(ProductContext context)
        {
            this.context = context;
        }

        public IQueryable<ItemEntity> GetAll()
        {
            return context.Items.AsQueryable();
        }

        public void Add(ItemEntity entity)
        {
            context.Entry(entity).State = EntityState.Added;
        }

        public void Update(ItemEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(ItemEntity entity)
        {
            context.Entry(entity).State = EntityState.Deleted;
        }
    }
}