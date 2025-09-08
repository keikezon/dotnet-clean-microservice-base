using Identity.Application.Users.Abstractions;
using Identity.Domain.Users;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken ct) => await db.Users.AddAsync(user, ct);

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        
        if (user == null)
            throw new Exception("User not found");
        
        return user;
    }

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    public async Task<User?> LoginAsync(Login login, CancellationToken ct)
    {
        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == login.Email, ct);

        if (user == null)
            throw new Exception("User not found");

        return user;
    }
}
