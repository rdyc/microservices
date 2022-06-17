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
            // builder.Ignore(e => e.Status);

            // define entity relation
            builder.HasOne(e => e.Attribute).WithMany(e => e.ProductAttributes).HasForeignKey(e => e.AttributeRefId).HasPrincipalKey(e => e.RefId).OnDelete(DeleteBehavior.Restrict);

            // define property mapping
            builder.Property(e => e.Id).IsRequired();
            builder.Property(e => e.ProductId).IsRequired();
            builder.Property(e => e.Sequence).IsRequired();
            builder.Property(e => e.AttributeRefId).IsRequired();
            builder.Property(e => e.Value).HasMaxLength(10).IsRequired();
        }
    }
}