using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Persistence.Configuration
{
    internal class AttributeConfiguration : IEntityTypeConfiguration<AttributeEntity>
    {
        public void Configure(EntityTypeBuilder<AttributeEntity> builder)
        {
            // schema & name
            builder.ToTable("attribute", "config");

            // ignoring some properties
            builder.Ignore(e => e.IsTransient);

            // define primary key
            builder.HasKey(e => e.Id);

            // define property mapping
            builder.Property(e => e.Id).IsRequired();
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Type).IsRequired();
            builder.Property(e => e.Unit).HasMaxLength(10).IsRequired();
            builder.Property(e => e.Value).HasMaxLength(10).IsRequired();
        }
    }
}