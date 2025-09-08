namespace Product.Application.Contracts.Events;

public record OrderCreatedIntegrationEvent(Guid OrderId, Guid ProductId, int Quantity);