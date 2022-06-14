using System.Threading;
using System.Threading.Tasks;
using Product.Domain.Context;

namespace Product.Domain.Repository
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ProductContext context;
        private bool isDisposed;

        public UnitOfWork(ProductContext context)
        {
            this.context = context;

            ProductItem = new ItemRepository(context);
        }

        public IItemRepository ProductItem { get; set; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}