using Common.Messaging;

namespace Identity.Contracts.Events;

public sealed class UserLoginIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public string Email { get; }

    public UserLoginIntegrationEvent(string email)
    {
        Email = email;
    }
}