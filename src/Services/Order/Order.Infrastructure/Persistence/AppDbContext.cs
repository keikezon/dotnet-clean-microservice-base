using Microsoft.EntityFrameworkCore;
using Order.Domain.Orders;

namespace Order.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<OrderModel> Orders => Set<OrderModel>();
    public DbSet<OrderItemModel> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}