using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Persistence.Configuration
{
    internal class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
    {
        public void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            // schema & name
            builder.ToTable("main", "product");

            // ignoring some properties
            builder.Ignore(e => e.IsTransient);

            // define primary key
            builder.HasKey(e => e.Id);

            // define entity relation
            // builder.HasMany(e => e.Attributes).WithOne(e => e.Product).HasForeignKey(e => e.ProductId).HasPrincipalKey(e => e.Id).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(e => e.Attributes).WithOne(e => e.Product).HasForeignKey(e => e.ProductId).HasPrincipalKey(e => e.Id).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Currency).WithOne(e => e.Product).HasForeignKey<ProductCurrencyEntity>(e => e.ProductId).OnDelete(DeleteBehavior.Restrict);

            // define property mapping
            builder.Property(e => e.Id).IsRequired();
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Description).HasMaxLength(150).IsRequired();
            builder.Property(e => e.Price).IsRequired();
        }
    }
}