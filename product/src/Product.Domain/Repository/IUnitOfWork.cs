using System;
using System.Threading;
using System.Threading.Tasks;

namespace Product.Domain.Repository
{
    internal interface IUnitOfWork : IDisposable
    {
        IItemRepository ProductItem { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}