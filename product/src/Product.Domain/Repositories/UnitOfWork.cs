using System.Threading;
using System.Threading.Tasks;
using Product.Domain.Persistence;

namespace Product.Domain.Repositories
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ProductContext context;
        private bool isDisposed;

        public UnitOfWork(ProductContext context)
        {
            this.context = context;

            Product = new ProductRepository(context);
        }

        public IProductRepository Product { get; set; }

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