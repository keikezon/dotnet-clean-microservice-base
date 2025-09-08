using Identity.Application.Users.Abstractions;
using Identity.Contracts.Events;
using Identity.Domain.Users;
using MassTransit;

namespace Identity.Application.Users.Commands;

public sealed class GetUser
{
    public sealed record Command(Guid Id);

    public interface IHandler
    {
        Task<User?> Handle(Command cmd, CancellationToken ct);
    }

    public sealed class Handler(IUserRepository repo) : IHandler
    {
        public async Task<User?> Handle(Command cmd, CancellationToken ct)
        {
            if (cmd.Id == Guid.Empty)
                throw new ArgumentException("Invalid Id.");
            
            var user = await repo.GetByIdAsync(cmd.Id, ct);
            
            return user;
        }
    }
}