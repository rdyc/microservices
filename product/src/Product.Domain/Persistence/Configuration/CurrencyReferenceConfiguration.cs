using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Persistence.Configuration
{
    internal class CurrencyReferenceConfiguration : IEntityTypeConfiguration<CurrencyReferenceEntity>
    {
        public void Configure(EntityTypeBuilder<CurrencyReferenceEntity> builder)
        {
            // schema & name
            builder.ToTable("currency", "reference");

            // ignoring some properties
            // builder.Ignore(e => e.Status);

            // define primary key
            builder.HasKey(e => e.RefId);

            // define indexes
            builder.HasIndex(e => new { e.Id, e.Code, e.Name, e.Symbol });

            // define property mapping
            builder.Property(e => e.RefId).IsRequired();
            builder.Property(e => e.Id).IsRequired();
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Code).HasMaxLength(3).IsRequired();
            builder.Property(e => e.Symbol).HasMaxLength(5).IsRequired();
        }
    }
}