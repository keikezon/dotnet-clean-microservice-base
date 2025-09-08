using Microsoft.EntityFrameworkCore;
using Product.Domain.Products;

namespace Product.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ProductModel> Products => Set<ProductModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<ProductModel>().HasData(new ProductModel(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Churu de frango",
            "Patê para gatos",
            25,
            10
        ));
        
        modelBuilder.Entity<ProductModel>().HasData(new ProductModel(
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            "Churu de carne",
            "Patê para gatos",
            25,
            10
        ));
    }
}