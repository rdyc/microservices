using System;
using System.Threading;
using System.Threading.Tasks;

namespace Product.Domain.Repositories
{
    internal interface IUnitOfWork : IDisposable
    {
        IProductRepository Product { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}