using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Product.Domain.Persistence;
using Shared.Infrastructure.Domain;

namespace Product.Domain.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ProductContext context;
    private bool isDisposed;

    public UnitOfWork(ProductContext context)
    {
        this.context = context;

        Config = new ConfigRepository(context);
        Product = new ProductRepository(context);
        Reference = new ReferenceRepository(context);
    }

    public IConfigRepository Config { get; set; }
    public IProductRepository Product { get; set; }
    public IReferenceRepository Reference { get; set; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        var result = await context.SaveChangesAsync(cancellationToken);

        DispatchEvents();

        return result;
    }

    private void DispatchEvents()
    {
        var entities = context.ChangeTracker.Entries<IEntity>()
            .Select(po => po.Entity)
            .Where(po => po.Events.Any())
            .ToArray();

        foreach (var entity in entities)
        {
            var events = entity.Events.ToArray();
            entity.Events.Clear();

            foreach (var domainEvent in events)
            {
                // TODO
                // _dispatcher.Dispatch(domainEvent);
                System.Console.WriteLine($"dispatch event {domainEvent.GetType().Name} with aggregate id: {domainEvent.AggregateId} at {domainEvent.Time}");
            }
        }
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