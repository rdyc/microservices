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
            builder.ToTable("main", "products");

            // ignoring some properties
            builder.Ignore(e => e.IsTransient);

            // define primary key
            builder.HasKey(e => e.Id);
            // builder.HasIndex(e => new { e.NameNormalized });

            // define entity relation
            // builder.HasMany(e => e.Addresses).WithOne(e => e.Client).HasForeignKey(e => e.ClientId).HasPrincipalKey(e => e.Id).OnDelete(DeleteBehavior.Restrict);
            // builder.HasMany(e => e.Contacts).WithOne(e => e.Client).HasForeignKey(e => e.ClientId).HasPrincipalKey(e => e.Id).OnDelete(DeleteBehavior.Restrict);
            // builder.HasMany(e => e.Maintainers).WithOne(e => e.Client).HasForeignKey(e => e.ClientId).HasPrincipalKey(e => e.Id).OnDelete(DeleteBehavior.Restrict);

            // builder.HasOne(e => e.CreatedUser).WithMany(e => e.CreatedClients).HasForeignKey(e => e.CreatedBy).HasPrincipalKey(e => e.RefId).OnDelete(DeleteBehavior.Restrict);
            // builder.HasOne(e => e.UpdatedUser).WithMany(e => e.UpdatedClients).HasForeignKey(e => e.UpdatedBy).HasPrincipalKey(e => e.RefId).OnDelete(DeleteBehavior.Restrict);
            // builder.HasOne(e => e.DeletedUser).WithMany(e => e.DeletedClients).HasForeignKey(e => e.DeletedBy).HasPrincipalKey(e => e.RefId).OnDelete(DeleteBehavior.Restrict);

            // builder.HasOne(e => e.Company).WithMany(e => e.Clients).HasForeignKey(e => e.CompanyRefId).HasPrincipalKey(e => e.RefId).OnDelete(DeleteBehavior.Restrict);

            // define property mapping
            builder.Property(e => e.Id).IsRequired();
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Description).HasMaxLength(150).IsRequired();
        }
    }
}