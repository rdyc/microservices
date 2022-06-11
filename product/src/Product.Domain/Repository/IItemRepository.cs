using System.Linq;
using Product.Domain.Entity;

namespace Product.Domain.Repository
{
    internal interface IItemRepository
    {
        IQueryable<ItemEntity> GetAll();
        void Add(ItemEntity entity);
        void Update(ItemEntity entity);
        void Delete(ItemEntity entity);
    }
}