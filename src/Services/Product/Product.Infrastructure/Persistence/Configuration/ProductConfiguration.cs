using Product.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Product.Infrastructure.Persistence.Configuration;

public sealed class ProductConfiguration : IEntityTypeConfiguration<ProductModel>
{
    public void Configure(EntityTypeBuilder<ProductModel> builder)
    {
        builder.ToTable("products");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Price).IsRequired();
        builder.Property(x => x.Stock).IsRequired();
        builder.Property(x => x.Invoice).HasMaxLength(500);
        builder.Property(x => x.Active).IsRequired();
    }
}
