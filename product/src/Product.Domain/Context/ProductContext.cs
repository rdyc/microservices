using Microsoft.EntityFrameworkCore;
using Product.Domain.Context.Configuration;
using Product.Domain.Entity;

namespace Product.Domain.Context
{
    internal class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ItemEntity> Items { get; set; }
        
        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Ignore<Event>();
            // modelBuilder.ApplyQueryFilter<ISoftDelete>(p => !p.DeletedAt.HasValue);

            modelBuilder.ApplyConfiguration(new ItemConfiguration());
        }
    }
}