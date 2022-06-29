using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Persistence.Configuration;

internal class AttributeReferenceConfiguration : IEntityTypeConfiguration<AttributeReferenceEntity>
{
    public void Configure(EntityTypeBuilder<AttributeReferenceEntity> builder)
    {
        // schema & name
        builder.ToTable("attribute", "reference");

        // ignoring some properties
        // builder.Ignore(e => e.Status);

        // define primary key
        builder.HasKey(e => e.RefId);

        // define indexes
        builder.HasIndex(e => new { e.Id, e.Name, e.Type, e.Unit });

        // define property mapping
        builder.Property(e => e.RefId).IsRequired();
        builder.Property(e => e.Id).IsRequired();
        builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Type).IsRequired();
        builder.Property(e => e.Unit).HasMaxLength(10).IsRequired();
    }
}