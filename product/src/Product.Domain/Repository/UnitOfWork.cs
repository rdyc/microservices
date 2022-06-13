using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Product.Domain.Context;
using Product.Domain.Entity;

namespace Product.Domain.Repository
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ProductContext context;
        private readonly ILogger<UnitOfWork> logger;
        private bool isDisposed;

        public UnitOfWork(ProductContext context, ILogger<UnitOfWork> logger)
        {
            this.context = context;
            this.logger = logger;

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