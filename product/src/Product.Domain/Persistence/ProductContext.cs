using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Product.Domain.Persistence.Configuration;
using Product.Domain.Persistence.Entities;
using Shared.Infrastructure.Domain;

namespace Product.Domain.Persistence;

internal class ProductContext : DbContext
{
    public ProductContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AttributeEntity> Attributes { get; set; }
    public DbSet<CurrencyEntity> Currencies { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<AttributeReferenceEntity> AttributeReferences { get; set; }
    public DbSet<CurrencyReferenceEntity> CurrencyReferences { get; set; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ignore any domain events
        modelBuilder.Ignore<IDomainEvent>();

        // applying query filter
        ApplyQueryFilter(modelBuilder);

        modelBuilder.ApplyConfiguration(new AttributeConfiguration());
        modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new ProductAttributeConfiguration());
        modelBuilder.ApplyConfiguration(new AttributeReferenceConfiguration());
        modelBuilder.ApplyConfiguration(new CurrencyReferenceConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    private static void ApplyQueryFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "p");
                var deletedCheck = Expression.Lambda(Expression.Equal(Expression.Property(parameter, "IsDeleted"), Expression.Constant(false)), parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(deletedCheck);
            }
        }
    }
}