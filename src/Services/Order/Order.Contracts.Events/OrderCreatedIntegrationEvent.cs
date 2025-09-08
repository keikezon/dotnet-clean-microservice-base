using Common.Messaging;

namespace Order.Contracts.Events;


public sealed class OrderCreatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public Guid OrderId { get; }

    public OrderCreatedIntegrationEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}