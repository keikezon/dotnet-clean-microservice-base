using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Orders;

namespace Order.Infrastructure.Persistence.Configuration;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItemModel>
{
    public void Configure(EntityTypeBuilder<OrderItemModel> builder)
    {
        builder.ToTable("orders_item");
        builder.HasKey(x => x.Id);
        builder.Property(e => e.ProductId);
        builder.Property(e => e.Quantity);
        builder.Property(e => e.UnitPrice);
        
    }
}