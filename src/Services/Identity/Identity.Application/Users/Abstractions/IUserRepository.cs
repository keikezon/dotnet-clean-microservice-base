using Identity.Domain.Users;

namespace Identity.Application.Users.Abstractions;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken ct);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<User?> LoginAsync(Login login, CancellationToken ct);
}
