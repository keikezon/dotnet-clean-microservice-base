using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Orders;

namespace Order.Infrastructure.Persistence.Configuration;

public sealed class OrderConfiguration : IEntityTypeConfiguration<OrderModel>
{
    public void Configure(EntityTypeBuilder<OrderModel> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(x => x.Id);
        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ClientDocument);
        builder.Property(e => e.TotalAmount);
        builder.HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey(e => e.OrderId);
        
    }
}