using Order.Application.Orders.Contracts;

namespace Order.Application.Orders.Abstractions;

public interface IIdentityClient
{
    Task<UserResponse?> GetByIdAsync(Guid userId, CancellationToken ct);
}