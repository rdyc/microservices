using System;
using System.Threading;
using System.Threading.Tasks;

namespace Product.Domain.Repositories
{
    internal interface IUnitOfWork : IDisposable
    {
        IConfigRepository Config { get; }
        IProductRepository Product { get; }
        IReferenceRepository Reference { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}