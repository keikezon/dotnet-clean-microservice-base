using Common.Abstractions;
using Common.Messaging;

namespace Identity.Contracts.Events;

public sealed class UserCreatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public Guid UserId { get; }
    public string Email { get; }

    public UserCreatedIntegrationEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}