using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Admin
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        modelBuilder.Entity<User>().HasData(new User(
            adminId,
            "Admin",
            "admin@email.com",
            "pbkdf2-256|mPafq1CXaNWHpPW3GGnhcg==|pU7K+StmlgzreQOEh1tf2AMTNH0GLj3OYTW8OTET1Go=", //admin@123
            "Admin"
        ));
    }
}
