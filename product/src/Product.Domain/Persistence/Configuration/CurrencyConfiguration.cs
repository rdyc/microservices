using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Persistence.Configuration;

internal class CurrencyConfiguration : IEntityTypeConfiguration<CurrencyEntity>
{
    public void Configure(EntityTypeBuilder<CurrencyEntity> builder)
    {
        // schema & name
        builder.ToTable("currency", "config");

        // ignoring some properties
        // builder.Ignore(e => e.Status);

        // define primary key
        builder.HasKey(e => e.Id);

        // define property mapping
        builder.Property(e => e.Id).IsRequired();
        builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Code).HasMaxLength(3).IsRequired();
        builder.Property(e => e.Symbol).HasMaxLength(5).IsRequired();
        builder.Property(e => e.IsDeleted).IsRequired();
    }
}