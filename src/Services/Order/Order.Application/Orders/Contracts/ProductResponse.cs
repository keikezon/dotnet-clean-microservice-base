namespace Order.Application.Orders.Contracts;

public sealed record ProductResponse(Guid Id, string Name, string Description, decimal Price, int Stock);