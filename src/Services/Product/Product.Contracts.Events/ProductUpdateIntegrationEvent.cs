using Common.Abstractions;
using Common.Messaging;

namespace Product.Application.Contracts.Events;

public sealed class ProductUpdateIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public Guid ProductId { get; }
    public string Name { get; }

    public ProductUpdateIntegrationEvent(Guid productId, string name)
    {
        ProductId = productId;
        Name = name;
    }
}