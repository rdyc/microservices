using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Persistence.Configuration
{
    internal class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttributeEntity>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeEntity> builder)
        {
            // schema & name
            builder.ToTable("attribute", "product");

            // define primary key
            builder.HasKey(e => e.Id);

            // ignoring some properties
            builder.Ignore(e => e.IsTransient);

            // define property mapping
            builder.Property(e => e.Id).IsRequired();
            builder.Property(e => e.ProductId).IsRequired();
            builder.Property(e => e.Sequence).IsRequired();
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Type).IsRequired();
            builder.Property(e => e.Value).HasMaxLength(10).IsRequired();
            builder.Property(e => e.Unit).HasMaxLength(10).IsRequired();
        }
    }
}