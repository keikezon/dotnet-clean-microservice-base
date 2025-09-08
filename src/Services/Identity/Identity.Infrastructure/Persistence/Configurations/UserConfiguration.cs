using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Profile).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired().HasDefaultValueSql("NOW()");
        builder.Property(x => x.Email).HasMaxLength(320).IsRequired();

        builder.Property(x => x.PasswordHash).HasMaxLength(300).IsRequired();
    }
}
