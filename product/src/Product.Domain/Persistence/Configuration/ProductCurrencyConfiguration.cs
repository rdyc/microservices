using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Persistence.Configuration
{
    internal class ProductCurrencyConfiguration : IEntityTypeConfiguration<ProductCurrencyEntity>
    {
        public void Configure(EntityTypeBuilder<ProductCurrencyEntity> builder)
        {
            // schema & name
            builder.ToTable("currency", "product");

            // ignoring some properties
            builder.Ignore(e => e.IsTransient);

            // define primary key
            builder.HasKey(e => e.ProductId);

            // define property mapping
            builder.Property(e => e.ProductId).IsRequired();
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Code).HasMaxLength(3).IsRequired();
            builder.Property(e => e.Symbol).HasMaxLength(5).IsRequired();
        }
    }
}