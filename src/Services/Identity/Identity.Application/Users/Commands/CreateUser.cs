using Common.Enum;
using Identity.Application.Users.Abstractions;
using Identity.Contracts.Events;
using Identity.Domain.Users;
using MassTransit;

namespace Identity.Application.Users.Commands;

public sealed class CreateUser
{
    public sealed record Command(string Name, string Email, string Password, UserProfile Profile);

    public interface IHandler
    {
        Task<Guid> Handle(Command cmd, CancellationToken ct);
    }

    public sealed class Handler(IUserRepository repo, IPasswordHasher hasher, IPublishEndpoint bus) : IHandler
    {
        public async Task<Guid> Handle(Command cmd, CancellationToken ct)
        {
            var user = User.Create(cmd.Name, cmd.Email, cmd.Password, cmd.Profile.ToString(), hasher);
            await repo.AddAsync(user, ct);
            await repo.SaveChangesAsync(ct);

            // Publica evento via contrato
            var @event = new UserCreatedIntegrationEvent(user.Id, user.Email);
            await bus.Publish(@event, ct);
            
            return user.Id;
        }
    }
}
